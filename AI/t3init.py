from transformers import pipeline
import sys

def initialclassify(text):
    classifier = pipeline("zero-shot-classification", model="facebook/bart-large-mnli")
    
    candidate_labels = ["High", "Medium", "Low"]
    result = classifier(text, candidate_labels)

    label = result['labels'][0].lower()
    return label

if __name__ == "__main__":
    text = sys.argv[1]
    prio = initialclassify(text)
    print(prio.capitalize())
#Unable to classify which is medium priority properly through this model
#This model is good enough for the current scenario but we need to be able to ckassify other instacnes as well .TO achieve this we need to train it with a respective buisness use case. 
#like if suppose we want to say that for a vendor who sells a product to an customer ,issues can be raised by the customer through this .But to identify priority we need to train our model with
#the respective dataset in accordance to the product . THus if supposed product is unable to run is labelled low and product crashed Fully should be labelled high . Or like security vulnerabilities shouls be labeled high
#
