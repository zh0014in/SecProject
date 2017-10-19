import numpy as np
import pandas as pd
from numpy.core.defchararray import index
from sklearn.model_selection import cross_val_score
from sklearn.model_selection import train_test_split
from sklearn.model_selection import GridSearchCV
from sklearn.metrics import classification_report
from sklearn.model_selection import cross_val_score
from sklearn.svm import SVC

train = pd.read_csv('../packets/train', sep=",")
test = pd.read_csv('../packets/test', sep=",")
data_train = np.array(train)
data_test = np.array(test)

X_train = data_train[:, 5:8]
y_train = data_train[:, 8]

print X_train
print y_train

X_test = data_test[:, 5:8]
X_check = data_test[:, 5:9]

clf = SVC(C=400, cache_size=2000, probability=True, gamma=0.01)
clf.fit(X_train, y_train)

result = clf.predict(X_test)
result = np.column_stack([X_check, result])

print result
thefile = open('result_svm.csv', 'w')
thefile.write("Length,PreviousLength,InterArrivalTime,Type,Predict\n")
for item in result:
    thefile.write("%s\n" % ",".join(str(x) for x in item))