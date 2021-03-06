import numpy as np
import pandas as pd
from numpy.core.defchararray import index
from sklearn.model_selection import cross_val_score
from sklearn.model_selection import train_test_split
from sklearn.model_selection import GridSearchCV
from sklearn.metrics import classification_report
from sklearn.model_selection import cross_val_score
from sklearn.svm import SVC
from sklearn.cluster import KMeans

train = pd.read_csv('../packets/train', sep=",")
data_train = np.array(train)

i_train = data_train[:, 0]
X = data_train[:, 5:6]
y = data_train[:, 7]

print X

kmeans = KMeans(init='k-means++', n_clusters=4, n_init=10)
scores = cross_val_score(kmeans, X, y)
print scores.mean()

# # Split the dataset in two equal parts
# X_train, X_test, y_train, y_test = train_test_split(
#     X, y, test_size=0.5, random_state=0)

# # Set the parameters by cross-validation
# tuned_parameters = [{'init': [ 'k-means++', 'random' ]}]

# scores = ['recall', 'precision']

# for score in scores:
#     print("# Tuning hyper-parameters for %s" % score)
#     print()
#     kmeans = KMeans(init='k-means++', n_clusters=4, n_init=10)
#     clf = GridSearchCV(kmeans, tuned_parameters, cv=5,
#                        scoring='%s_macro' % score, n_jobs=2)
#     clf.fit(X_train, y_train)

#     print("Best parameters set found on development set:")
#     print()
#     print(clf.best_params_)
#     print()
#     print("Grid scores on development set:")
#     print()
#     means = clf.cv_results_['mean_test_score']
#     stds = clf.cv_results_['std_test_score']
#     for mean, std, params in zip(means, stds, clf.cv_results_['params']):
#         print("%0.3f (+/-%0.03f) for %r"
#                 % (mean, std * 2, params))
#     print()

#     print("Detailed classification report:")
#     print()
#     print("The model is trained on the full development set.")
#     print("The scores are computed on the full evaluation set.")
#     print()
#     y_true, y_pred = y_test, clf.predict(X_test)
#     print(classification_report(y_true, y_pred))
#     print()
