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