# Unity Custom Events System

A flexible **Custom Event System for Unity** that extends the functionality of UnityEvents.  
Unlike UnityEvents, which are limited to a single parameter, this system allows you to invoke methods with **multiple parameters of different types** — directly from the Inspector.

## ✨ Features
- 🎯 Works just like UnityEvents, but with no single-parameter limitation.
- 📝 Inspector UI with **ReorderableList** for easy event management.
- 🔗 Assign **Target Object → Component → Method** from the editor.
- 🔄 Supports regular parameter types (bool, int, float, string) as well as Unity object types (AudioClip, MeshFilter, Material, etc).
- ⚡ Lightweight, reflection-based method calling.
- 🧑‍💻 Designer-friendly and requires **no extra code** to set up.

## 🚀 Usage
You can simply declare a public CustomEvent myEvents; field in your MonoBehaviour.
==> From there, you can trigger and invoke any function directly — with support for multiple parameters — using an easy, Unity-Event-like inspector UI.
