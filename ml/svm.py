import numpy as np
import pandas as pd
from sklearn.svm import SVC
import os.path
from sklearn.externals import joblib

train = pd.read_csv('../packets/train', sep=",")
test = pd.read_csv('../packets/test', sep=",")
data_train = np.array(train)
data_test = np.array(test)

X_train = data_train[:, 5:8]
y_train = data_train[:, 8]

print X_train
print y_train

X_test = data_test[:, 5:8]

if os.path.isfile('svm.pkl'):
    clf = joblib.load('svm.pkl')
else:
    clf = SVC(kernel='rbf', C=400, cache_size=2000, probability=True, gamma=0.01)
    clf.fit(X_train, y_train)

    # store trained model
    joblib.dump(clf, 'svm.pkl')

result = clf.predict(X_test)


result = np.column_stack([data_test, result])

print result
thefile = open('result_svm.csv', 'w')
thefile.write("Length,PreviousLength,InterArrivalTime,Type,Predict\n")
for item in result:
    thefile.write("%s\n" % ",".join(str(x) for x in item))