from flask import Flask, request, send_file, jsonify
from nltk import ne_chunk
from openai import OpenAI
from io import BytesIO
import requests

import spacy

import nltk
from nltk.tokenize import word_tokenize
from nltk.tag import pos_tag

nltk.download('averaged_perceptron_tagger')
nltk.download('punkt')
nltk.download('maxent_ne_chunker')
nltk.download('words')

nlp = spacy.load("en_core_web_sm")

import nltk
from nltk.tokenize import word_tokenize
from nltk.tag import pos_tag


def extract_important_features(text):
    """Extracts nouns, adjectives, named entities, and considers sentiment."""
    tokens = word_tokenize(text.lower())

    # Named Entity Recognition (optional)
    named_entities = ne_chunk(pos_tag(tokens))  # Extract named entities

    # Sentiment analysis (optional)
    # sentiment = analyze_sentiment(text)  # Replace with your sentiment analysis library

    important_words = []
    for word, tag in pos_tag(tokens):
        if (tag == 'NN' or tag == 'NNS') and len(word) > 3:  # Nouns (singular/plural)
            important_words.append(word)
        elif (tag == 'JJ' or tag == 'JJS' or tag == 'JJR') and len(word) > 3:  # Adjectives
            important_words.append(word)

    # Add named entities (optional)
    for entity in named_entities:
        if isinstance(entity, nltk.tree.Tree):
            entity_text = " ".join([word for word, tag in entity.leaves()])
            important_words.append(entity_text)

    # Consider sentiment (optional)
    # if sentiment == "positive":
    #   important_words.append("happy")
    # elif sentiment == "negative":
    #   important_words.append("sad")

    return " ".join(important_words)


app = Flask(__name__)

@app.route('/')
def home():
    return "Hello Flask"

@app.route('/generate-image', methods=['POST'])
def generate_image():
    try:
        prompt = request.form['prompt']
        print(f"Received prompt: {prompt}")

        #prompt = extract_important_features(prompt)

        #print(f"Processed prompt: {prompt}")

        client = OpenAI(
            api_key= #yourapikey,
        )

        response = client.images.generate(
            model="dall-e-2",
            prompt=prompt,
            size="512x512",
            quality="standard",
            n=1,
        )

        image_url = response.data[0].url
        print(f"Image URL: {image_url}")

        # Download the image
        image_response = requests.get(image_url)
        image = BytesIO(image_response.content)
        print("Image downloaded successfully")

        return send_file(image, mimetype='image/png')
    except Exception as e:
        print(f"An error occurred: {e}")
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run()