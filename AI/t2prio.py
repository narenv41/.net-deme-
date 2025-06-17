from transformers import pipeline
import sys
import torch

def classify_priority(text):
    model_name = "bhadresh-savani/distilbert-base-uncased-emotion"
    classifier = pipeline("text-classification", model=model_name, device=0 if torch.cuda.is_available() else -1)

    result = classifier(text)
    label = result[0]['label'].lower()

    # Map emotion to priority
    if label in ['anger', 'fear']:
        return "High"
    elif label in ['sadness', 'disgust']:
        return "Medium"
    else:
        return "Low"

if __name__ == "__main__":
    input_text = sys.argv[1]
    priority = classify_priority(input_text)
    print(priority)

#we need to be able to run this faster . FOr every message i type it should run through this model which detects emotion. BUt when initially creating the ticket we need to classify if the ticket has low priority or high priority dependind upon the user's descripton(Liket total outage:High, Application completely down:HIGH, similar to that we are supposed to be able to classify all the tickets initially , to do that we need an ai model which wil be able to classify what type of priorityty, we can either use pre-trained model or train it with respective to the company)
