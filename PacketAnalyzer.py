import fitz
import io
import re
import csv

print (fitz.__doc__)

ClientHello = "Client Hello"
ServerHello = "Server Hello"
ClientChangeCipherSpec = "Change Cipher Spec"
ServerAddress = "192.168.0.111"
ClientAddress = "192.168.0.168"


def parseFile(fileName, output):
    print("opening file " + fileName)
    doc = fitz.open(fileName)
    pageCount = doc.pageCount
    with open(output, 'w',newline="\n", encoding="utf-8") as file:
        writer = csv.writer(file)
        ts = 0
        for c in range(0, pageCount):
            page = doc.loadPage(c)
            text = page.getText("text")
            buf = io.StringIO(text)
            lines = buf.readlines()
            # remove first two lines and the last two lines
            lines = lines[2:-1] 
            # filter out only server/client messages
            lines = [l for l in lines if ClientAddress in l and ServerAddress in l]
            for line in lines:
                split = re.split(r'\s{2,}',line)
                split.append( float(split[0])-ts)
                ts = float(split[0])
                if(split[4].startswith(ClientHello)):
                    split[4] = "1"
                elif split[4].startswith(ServerHello):
                    split[4] = "2"
                elif(split[4].startswith(ClientChangeCipherSpec)):
                    split[4] = "3"
                else:
                    split[4] = "0"
                print(split)
                writer.writerow(split)

parseFile("capture.xps", "./training.csv");
parseFile("test.xps", "test.csv")