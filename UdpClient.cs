﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;

namespace VoxPopuli
{
	public class UdpClient : IDisposable
	{
		private readonly DatagramSocket _socket;
		private readonly BlockingCollection<Tuple<IpAddress, byte[]>> _incomingMessages;
		private readonly IpAddress _ipAddress;
		private readonly object _lock = new object();
		private bool _isBound;
		private bool _isBinding;

		public UdpClient(IpAddress ipAddress)
		{
			_ipAddress = ipAddress;
			_socket = new DatagramSocket();
			_socket.Control.DontFragment = true;
			_incomingMessages = new BlockingCollection<Tuple<IpAddress, byte[]>>();
			_socket.MessageReceived += HandleIncomingMessages;
		}

		public async Task SendAsync(byte[] data)
		{
			try
			{
				await Connect();

				using (var stream = await _socket.GetOutputStreamAsync(_ipAddress.Host, _ipAddress.ServiceName))
				{
					await stream.WriteAsync(data.AsBuffer(0, data.Length));
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
		}

		public bool TryGetIncomingMessage(out Tuple<IpAddress, byte[]> message)
		{
			return _incomingMessages.TryTake(out message, TimeSpan.FromMilliseconds(20));
		}

		public void Dispose()
		{
			_socket.Dispose();
		}

		private async Task Connect()
		{
			if (_isBound || _isBinding)
			{
				return;
			}

			lock (_lock)
			{
				if (_isBound || _isBinding)
				{
					return;
				}

				_isBinding = true;
			}

			var possibleConnectionProfiles = NetworkInformation.GetConnectionProfiles()
				.Where(p => p.IsWlanConnectionProfile && p.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
				.ToList();

			var connectionProfile = possibleConnectionProfiles.FirstOrDefault();

			if (connectionProfile != null)
			{
				await _socket.BindServiceNameAsync(_ipAddress.ServiceName, connectionProfile.NetworkAdapter);
			}

			_isBound = true;
			_isBinding = false;
		}

		private void HandleIncomingMessages(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs e)
		{
			var address = e.RemoteAddress.ToString();
			var port = int.Parse(e.RemotePort);

			using (var reader = e.GetDataReader())
			{
				var data = reader.DetachBuffer().ToArray();
				_incomingMessages.Add(Tuple.Create(new IpAddress(address, port), data));
			}
		}
	}

	public struct IpAddress
	{
		public static readonly IpAddress Broadcast = new IpAddress("255.255.255.255", 0);

		public readonly string Address;
		public readonly int Port;
		public readonly HostName Host;
		public readonly string ServiceName;

		public IpAddress(string address, int port)
		{
			Address = address;
			Port = port;
			Host = new HostName(address);
			ServiceName = port.ToString(CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Address, Port);
		}

		public bool Equals(IpAddress other)
		{
			return string.Equals(Address, other.Address) && Port == other.Port;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is IpAddress && Equals((IpAddress)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Address != null ? Address.GetHashCode() : 0) * 397) ^ Port;
			}
		}
	}
	public static class IpProvider
	{
		private static readonly Lazy<string> _localAddress;
		private static readonly Lazy<string> _broadcastAddress;

		static IpProvider()
		{
			_localAddress = new Lazy<string>(GetLocalAddress);
			_broadcastAddress = new Lazy<string>(GetBroadcastAddress);
		}

		public static string LocalAddress { get { return _localAddress.Value; } }

		public static string BroadcastAddress { get { return _broadcastAddress.Value; } }

		private static string GetLocalAddress()
		{
			var hostnames = NetworkInformation.GetHostNames();
			foreach (var hn in hostnames)
			{
				//IanaInterfaceType == 71 => Wifi
				//IanaInterfaceType == 6 => Ethernet (Emulator)
				if (hn.IPInformation != null &&
					(hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71
					 || hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6))
				{
					return hn.DisplayName;
				}
			}

			return IpAddress.Broadcast.Address;
		}

		private static string GetBroadcastAddress()
		{
			var parts = _localAddress.Value.Split(new[] { '.' }).Take(3);
			return string.Join(".", parts) + ".255";
		}
	}
}