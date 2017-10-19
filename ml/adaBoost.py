import numpy as np
import pandas as pd
from sklearn.model_selection import cross_val_score
from sklearn.ensemble import AdaBoostClassifier

train = pd.read_csv('../packets/train', sep=",")
data_train = np.array(train)

i_train = data_train[:, 0]
X = data_train[:, 5:6]
y = data_train[:, 7]

clf = AdaBoostClassifier(n_estimators=100)
scores = cross_val_score(clf, X, y)
print scores.mean()