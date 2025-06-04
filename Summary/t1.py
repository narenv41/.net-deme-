import sys
import json
from transformers import pipeline

# Load model once (you can optimize with caching later)
summarizer = pipeline("summarization", model="facebook/bart-large-cnn")

# Read JSON from stdin
raw = sys.stdin.read()
data = json.loads(raw)

full_text = f"Ticket: {data['Title']}\nDescription: {data['Description']}\nMessages:\n" + "\n".join(data["Messages"])

summary = summarizer(full_text, max_length=130, min_length=30, do_sample=False)
print("Summary:", summary[0]['summary_text'])


