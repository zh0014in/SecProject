import numpy as np
import pandas as pd
from numpy.core.defchararray import index
from sklearn.model_selection import cross_val_score
from sklearn.model_selection import train_test_split
from sklearn.model_selection import GridSearchCV
from sklearn.metrics import classification_report
from sklearn.model_selection import cross_val_score
from sklearn.svm import SVC
from sklearn.ensemble import AdaBoostClassifier
from sklearn.naive_bayes import GaussianNB

train = pd.read_csv('../packets/train', sep=",")
data_train = np.array(train)

i_train = data_train[:, 0]
X = data_train[:, 5:6]
y = data_train[:, 7]

print X

clf = SVC(C=100, cache_size=2000, probability=True)
scores = cross_val_score(clf, X, y)
print scores.mean()

clf = AdaBoostClassifier(n_estimators=100)
scores = cross_val_score(clf, X, y)
print scores.mean()

gnb = GaussianNB()
y_pred = gnb.fit(X, y).predict(X)
print("Number of mislabeled points out of a total %d points : %d"
      % (X.shape[0],(y != y_pred).sum()))
