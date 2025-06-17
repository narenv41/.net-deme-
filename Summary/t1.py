import sys
import json
from transformers import pipeline

summarizer = pipeline("summarization", model="facebook/bart-large-cnn")

raw = sys.stdin.read()
data = json.loads(raw)

full_text = f"Ticket: {data['Title']}\nDescription: {data['Description']}\nMessages:\n" + "\n".join(data["Messages"])

summary = summarizer(full_text, max_length=130, min_length=30, do_sample=False)
print("Summary:", summary[0]['summary_text'])

# This summary model given by Facebook is not optimised for a ticketing system, it is recommended to use a model form Google, like Gemini or OpenAI. THis model is unable to summarize contents properly.
#best to utilise by buying OpenAI model, we can access them through API links 
