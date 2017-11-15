# SecProject
PhpServer - php application with a login page, need Internet Information Service (IIS) in windows or LAMP in linux to host<br/><br/>
Client - C# client to post data to the phpserver<br/><br/>
PacketAnalyzer - C# program to pre-process machine larning data, generate ssl sessions and MSC, PAT Model<br/><br/>
Report - reports<br/><br/>
SslTcpClient & SslTcpServer - SSL clients and servers to establish simple ssl connections<br/><br/>
certificates - the self-signed certificates used for server setup, you need to create your own, used by the SslTcpServer<br/><br/>
ml - machine learning code, use svm-tune.py to tune the parameters of svm, use svm.py to train the model and predict the labels, scores.py gives the scores of different algorithms<br/><br/>
models - the CSP models, tlsv1_0 is for tls, mitm for the attack<br/><br/>
msc - msc code and the generated images<br/><br/>
mscgen - the tool used to draw the msc images<br/><br/>
packets - the captured packages, use wireshark to open them ,and put ssl as the filter<br/><br/>
To Setup the attacks:<br/>
1. Install Kali linux and Bettercap<br/>
2. Install server and client vm, setup the php server<br/>
3. Run command in Kali 'bettercap --proxy-https --sniffer -G serverIP -T clientIP',<br/>
4. Run wireshark in Kali and put 'ssl' in the filter bar<br/>
5. Access the server's address in client and type in username, password, then login, Kali bettercap should have sniffed the password.
