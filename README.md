# About

* Render feature to blur the camera color using gaussian blur, integrated with URP volume system.
* Supported Unity versions are 2022.3, 2023.1, 2023.2 and Unity 6. Unity 6 has both rendergraph and compatibility mode support.

# Usage

* Add the fullscreen blur renderer feature to you URP Renderer.
* Enable post-processing both in your camera and URP Renderer.
* Add a volume to your scene and add override: "Custom->Fullscreen Blur".
* Animate the progress parameter to your needs to simulate a progressive blur.