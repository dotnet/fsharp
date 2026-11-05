"""Public child tree with socket-controlled lifetime, including an exiting wrapper."""

import socket
import subprocess
import sys

port, depth, mode, streams = sys.argv[1:]
depth = int(depth)
child = None
if depth:
    output = subprocess.DEVNULL if streams == "detached" else None
    child = subprocess.Popen([sys.executable, __file__, port, str(depth - 1), mode, streams],
                             cwd=sys.prefix, stdout=output, stderr=output)
with socket.create_connection(("127.0.0.1", int(port))) as connection:
    connection.sendall(str(depth).encode())
    while connection.recv(1) == b"p":
        connection.sendall(b"p")
if depth == 2 and mode != "timeout":
    sys.exit(7 if mode == "failure" else 0)
if child:
    child.wait()
