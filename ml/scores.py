import numpy as np
import pandas as pd
from sklearn.model_selection import cross_val_score
from sklearn.svm import SVC
from sklearn.ensemble import AdaBoostClassifier
from sklearn.naive_bayes import GaussianNB
from sklearn import neighbors

train = pd.read_csv('../packets/train', sep=",")
data_train = np.array(train)

X = data_train[:, 5:8]
y = data_train[:, 8]

print X
print y

# svm
clf = SVC(C=100, cache_size=2000, probability=True)
scores = cross_val_score(clf, X, y)
print scores.mean()

# AdaBoost
clf = AdaBoostClassifier(n_estimators=100)
scores = cross_val_score(clf, X, y)
print scores.mean()

# Naive Bayes
gnb = GaussianNB()
scores = cross_val_score(gnb, X, y)
print scores.mean()

# kNN
clf = neighbors.KNeighborsClassifier(15, weights='uniform')
scores = cross_val_score(clf, X, y)
print scores.mean()