using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eric.Morrison
{
    public class DatagramSocket : IDisposable
    {
        //
        // Summary:
        //     Creates a new DatagramSocket object.
        public DatagramSocket()
        { }

        //
        // Summary:
        //     Gets socket information on the local and remote hostnames and local and remote
        //     service names for the DatagramSocket object.
        //
        // Returns:
        //     Socket information for the DatagramSocket object.
        //public DatagramSocketInformation Information { get; }
        //
        // Summary:
        //     Gets the output stream to write to the remote host.
        //
        // Returns:
        //     A stream of bytes to be written to the remote host in a UDP datagram.

        public IOutputStream OutputStream { get; }

        //
        // Summary:
        //     An event that indicates that a message was received on the DatagramSocket object.
        public event TypedEventHandler<DatagramSocket, DatagramSocketMessageReceivedEventArgs> MessageReceived;

        //
        // Summary:
        //     Gets a list of EndpointPair objects based on a remote hostname and remote service
        //     name that can be used to send datagrams to a remote network destination.
        //
        // Parameters:
        //   remoteHostName:
        //     The remote hostname or IP address.
        //
        //   remoteServiceName:
        //     The remote service name or UDP port.
        //
        // Returns:
        //     A list of EndpointPair objects.
        [Overload("GetEndpointPairsAsync")]
        public static IAsyncOperation<IReadOnlyList<EndpointPair>> GetEndpointPairsAsync(HostName remoteHostName, System.String remoteServiceName);
        //
        // Summary:
        //     Gets a list of EndpointPair objects based on a remote hostname and remote service
        //     name and the sort order to be used.
        //
        // Parameters:
        //   remoteHostName:
        //     The remote hostname or IP address.
        //
        //   remoteServiceName:
        //     The remote service name or UDP port.
        //
        //   sortOptions:
        //     The sort order to use when returning the list.
        //
        // Returns:
        //     A list of EndpointPair objects.
        [Overload("GetEndpointPairsWithSortOptionsAsync")]
        public static IAsyncOperation<IReadOnlyList<EndpointPair>> GetEndpointPairsAsync(HostName remoteHostName, System.String remoteServiceName, HostNameSortOptions sortOptions);
        //
        // Summary:
        //     Starts a bind operation on a DatagramSocket to a local hostname and a local service
        //     name.
        //
        // Parameters:
        //   localHostName:
        //     The local hostname or IP address on which to bind the DatagramSocket object.
        //
        //   localServiceName:
        //     The local service name or UDP port on which to bind the DatagramSocket object.
        //
        // Returns:
        //     An asynchronous bind operation on a DatagramSocket object.
        public IAsyncAction BindEndpointAsync(HostName localHostName, System.String localServiceName);
        //
        // Summary:
        //     Starts a bind operation on a DatagramSocket to a local service name.
        //
        // Parameters:
        //   localServiceName:
        //     The local service name or UDP port on which to bind the DatagramSocket object.
        //
        // Returns:
        //     An asynchronous bind operation on a DatagramSocket object.
        public IAsyncAction BindServiceNameAsync(System.String localServiceName);
        //
        // Summary:
        //     Starts a bind operation on a DatagramSocket to a local service name and specific
        //     network interface.
        //
        // Parameters:
        //   localServiceName:
        //     The local service name or UDP port on which to bind the DatagramSocket object.
        //
        //   adapter:
        //     The network adapter on which to bind the DatagramSocket object.
        //
        // Returns:
        //     An asynchronous bind operation on a DatagramSocket object.
        [Overload("BindServiceNameAndAdapterAsync")]
        public IAsyncAction BindServiceNameAsync(System.String localServiceName, NetworkAdapter adapter);
        //
        // Summary:
        //     Starts a connect operation on a DatagramSocket to a remote network destination
        //     specified as an EndpointPair object.
        //
        // Parameters:
        //   endpointPair:
        //     An EndpointPair object that specifies local hostname or IP address, local service
        //     name or UDP port, the remote hostname or remote IP address, and the remote service
        //     name or remote UDP port for the remote network destination.
        //
        // Returns:
        //     An asynchronous connect operation on a DatagramSocket object.
        [Overload("ConnectWithEndpointPairAsync")]
        public IAsyncAction ConnectAsync(EndpointPair endpointPair);
        //
        // Summary:
        //     Starts a connect operation on a DatagramSocket to a remote destination specified
        //     by a remote hostname and a remote service name.
        //
        // Parameters:
        //   remoteHostName:
        //     The hostname or IP address of the remote network destination.
        //
        //   remoteServiceName:
        //     The service name or UDP port of the remote network destination.
        //
        // Returns:
        //     An asynchronous connect operation on a DatagramSocket object.
        [Overload("ConnectAsync")]
        public IAsyncAction ConnectAsync(HostName remoteHostName, System.String remoteServiceName);
        //
        // Summary:
        //     Performs tasks associated with freeing, releasing, or resetting unmanaged resources
        //     on the DatagramSocket object and aborts any pending operation on the DatagramSocket.
        public void Dispose();
        //
        // Summary:
        //     Starts an operation to get an IOutputStream to a remote network destination specified
        //     by an EndpointPair object that can then be used to send network data.
        //
        // Parameters:
        //   endpointPair:
        //     An endpoint pair that represents the local hostname or local IP address, the
        //     local service name or local UDP port, the remote hostname or remote IP address,
        //     and the remote service name or remote UDP port.
        //
        // Returns:
        //     An IOutputStream that represents the asynchronous operation.
        [Overload("GetOutputStreamWithEndpointPairAsync")]
        public IAsyncOperation<IOutputStream> GetOutputStreamAsync(EndpointPair endpointPair);
        //
        // Summary:
        //     Starts an operation to get an IOutputStream to a remote destination specified
        //     by a remote hostname and a remote service name that can then be used to send
        //     network data.
        //
        // Parameters:
        //   remoteHostName:
        //     The remote hostname or remote IP address.
        //
        //   remoteServiceName:
        //     The remote service name or remote UDP port.
        //
        // Returns:
        //     An IOutputStream that representing the asynchronous operation.
        [Overload("GetOutputStreamAsync")]
        public IAsyncOperation<IOutputStream> GetOutputStreamAsync(HostName remoteHostName, System.String remoteServiceName);
        //
        // Summary:
        //     Joins a DatagramSocket object to a multicast group.
        //
        // Parameters:
        //   host:
        //     The hostname or IP address for the multicast group.
        public void JoinMulticastGroup(HostName host);
    }
}