import numpy as np
import pandas as pd
from numpy.core.defchararray import index
from sklearn.model_selection import cross_val_score

train = pd.read_csv('../packets/train',sep=",")
data_train = np.array(train)

i_train = data_train[:, 0]
X_train = data_train[:, 5:6]  
y_train = data_train[:, 7]

print X_train

from sklearn import svm
    
c = 500.0
g = 0.01
k = 'rbf'
clf = svm.SVC(C=c, gamma=g, kernel=k)
scores = cross_val_score(clf, iris.data, iris.target, cv=5)

print clf.fit(X_train,y_train)

score = clf.score(X_test,y_test)
print(score)

# result = clf.predict(X_test)
#  
# result = np.column_stack([i_test, result])
# result = np.vstack([p_train, result])
# print result
# thefile = open('result.csv', 'w')
# thefile.write("Id,Prediction\n")
# for index, item in enumerate(result):
#     thefile.write("%s" % item[0])
#     thefile.write(",%s\n" % item[1])

# a = np.concatenate((i_train,i_test))
# print a
# for index,value in enumerate(a):
#     print value
    
# print(i_train)
# a = np.concatenate((i_train, y_train),axis = 0)
# print(a)
  
#===============================================================================
# thefile = open('result.csv', 'w')
# thefile.write("Id,Prediction\n")
# errorCount = 0
# for index, item in enumerate(result):
#     thefile.write("%s" % (index+1))
#     thefile.write(",%s" % item)
#     thefile.write(",%s\n" % y_train[index])
#     if item != y_train[index]:
#         errorCount = errorCount+1
#           
# thefile.write("%s, " % errorCount)
# thefile.write("%s\n" % len(result))
#===============================================================================
