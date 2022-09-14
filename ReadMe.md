## Contents
* [Download the App](#download)
* [About ToF AR](#about)
* [Overview of ToF AR Samples Basic](#overview)
* [Component](#component)
* [Assets](#assets)
* [Development environment](#environment)
* [Contributing](#contributing)

<a name="download"></a>
# Download the App

Experience immersive AR with ToF AR.  
You can find the App from the stores below.

[<img alt="Get it on the App Store" src="/Docs/images/App_Store_Badge_US-UK_092917.svg" height="60">](https://apps.apple.com/us/developer/id1601362415)
&nbsp;&nbsp;&nbsp;&nbsp;
[<img alt="Get it on Google Play" src="/Docs/images/google-play-badge_us.png" height="70">](https://play.google.com/store/apps/developer?id=Sony+Semiconductor+Solutions+Corporation)


<a name="about"></a>
# About ToF AR

ToF AR, Time of Flight Augmented Reality, is a toolkit library intended to aid in Unity application development for iOS and Android devices. ToF AR ToF AR mainly targets iOS / Android smartphones with ToF sensor, but it also works with Depth sensors other than ToF, such as Structured light method sensor.

As well as this ToF AR, Unity and compatible devices with ToF sensors are required to build and execute this sample application.

Please see [the ToF AR Site on Developer World](https://developer.sony.com/develop/tof-ar) for ToF AR downloads and development guides, sample applications, and a list of compatible devices.


<a name="overview"></a>
# Overview of ToF AR Samples Basic

**ToF AR Samples Basic** is a simple sample application that uses the functions of ToF AR and provides the following 17 scenes:


## List of scenes

<table>
<tr align="center">
    <th width="250">Color</th>
    <th width="250">ColorDepth</th>
    <th width="250">BasicStream</th>
    <th width="250">DepthConfidence</th>
</tr>
<tr align="center">
    <td>Display Color image</td>
    <td>Superimpose Color image and Depth image</td>
    <td>Tile Depth, Confidence and Color image</td>
    <td>Display Depth image and Confidence image side by side</td>
</tr>
<tr align="center">
    <td><img src="/Docs/images/01_Color.jpg" width="150"></td>
    <td><img src="/Docs/images/02_ColorDepth.jpg" width="150"></td>
    <td><img src="/Docs/images/03_BasicStream.jpg" width="150"></td>
    <td><img src="/Docs/images/04_DepthConfidence.jpg" width="150"></td>
</tr>
</table>

<table>
<tr align="center">
    <th width="250">PointCloud</th>
    <th width="250">ColoredPointCloud</th>
    <th width="250">HumanPointCloud</th>
    <th width="250">Segmentation</th>
</tr>
<tr align="center">
    <td>Display a subject as a single color PointCloud</td>
    <td>Display a subject as a color PointCloud</td>
    <td>Display a human body as a color PointCloud</td>
    <td>Display segmentation results of person and sky as masks</td>
</tr>
<tr align="center">
    <td><img src="/Docs/images/05_PointCloud.jpg" width="150"></td>
    <td><img src="/Docs/images/06_ColoredPointCloud.jpg" width="150"></td>
    <td><img src="/Docs/images/07_HumanPointCloud.jpg" width="150"></td>
    <td><img src="/Docs/images/08_Segmentation.jpg" width="150"></td>
</tr>
</table>

<table>
<tr align="center">
    <th width="250">Hand</th>
    <th width="250">LiveMeshOcclusion</th>
    <th width="250">ColorHandOcclusion</th>
    <th width="250">Body</th>
</tr>
<tr align="center">
    <td>Display a hand bone model</td>
    <td>Display a virtual cube occluded by the subject based on distance measurement</td>
    <td>Display a virtual cube occluded by the hand based on distance measurement, and display hand bone model</td>
    <td>Display a human bone model on the Depth image</td>
</tr>
<tr align="center">
    <td><img src="/Docs/images/09_Hand.jpg" width="150"></td>
    <td><img src="/Docs/images/10_LiveMeshOcclusion.jpg" width="150"></td>
    <td><img src="/Docs/images/11_ColorHandOcclusion.jpg" width="150"></td>
    <td><img src="/Docs/images/12_Body.jpg" width="150"></td>
</tr>
</table>

<table>
<tr align="center">
    <th width="250">ColorBody</th>
    <th width="250">HandMark</th>
    <th width="250">ColorHandMark</th>
    <th width="250">SLAM</th>
</tr>
<tr align="center">
    <td>Display a human bone model on superimposed Depth and Color image</td>
    <td>Recognize the mark drawn with the index finger and display the mark name</td>
    <td>Recognize the mark drawn with the index finger and display the mark name. Superimpose Color image and Depth image</td>
    <td>Display the coordinates of the SLAM-supported camera</td>
</tr>
<tr align="center">
    <td><img src="/Docs/images/13_ColorBody.jpg" width="150"></td>
    <td><img src="/Docs/images/14_HandMark.jpg" width="150"></td>
    <td><img src="/Docs/images/15_ColorHandMark.jpg" width="150"></td>
    <td><img src="/Docs/images/16_SLAM.jpg" width="150"></td>
</tr>
</table>

<table>
<tr align="center">
    <th width="250">Face</th>
</tr>
<tr align="center">
    <td>Overlay white mask on the face based on the face recognition. Also display the recognition result from the shape of the mouth</td>
</tr>
<tr align="center">
    <td><img src="/Docs/images/17_Face.jpg" width="150"></td>
</tr>
</table>

## Operation

Using the built application is described below.

1. When you start the application, the scene list is displayed on the screen. Select a scene from the list and tap it to start it.

    <img src="/Docs/images/topmenu.jpg" width="150">

1. Tap the camera icon at the bottom left of the screen to display a list of available cameras for the selected scene. Select an appropriate camera, and the scene will start working.

1. Tap the screen with four fingers to return to the top screen.


<a name="component"></a>
# Component

The table below shows the relationships between the 17 scenes in the sample application and the ToF AR components used by each scene. The scene names are arranged vertically and the component names are arranged horizontally. A check mark indicates that the component is used.


||ToF|Color|Mesh|Coordinate|Hand|MarkRecog|Body|Segmentation|Slam|Face|Plane|Modeling|
|:--|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
|Color             |  |✓|  |  |  |  |  |  |  |  |  |  |
|ColorDepth        |✓|✓|  |✓|  |  |  |  |  |  |  |  |
|BasicStream       |✓|✓|  |  |  |  |  |  |  |  |  |  |
|DepthConfidence   |✓|  |  |  |  |  |  |  |  |  |  |  |
|PointCloud        |✓|  |  |  |  |  |  |  |  |  |  |  |
|ColorPointCloud   |✓|✓|  |✓|  |  |  |  |  |  |  |  |
|HumanPointCloud   |✓|✓|  |✓|  |  |  |✓|  |  |  |  |
|Segmentation      |  |✓|  |  |  |  |  |✓|  |  |  |  |
|Hand              |✓|✓|  |✓|✓|  |  |  |  |  |  |  |
|LiveMeshOcclusion |✓|✓|✓|✓|  |  |  |  |  |  |  |  |
|ColorHandOcclusion|✓|✓|✓|✓|✓|  |  |  |  |  |  |  |
|Body              |✓|  |  |  |  |  |✓|  |  |  |  |  |
|ColorBody         |✓|✓|  |✓|  |  |✓|  |  |  |  |  |
|HandHark          |✓|  |  |  |✓|✓|  |  |  |  |  |  |
|ColorHandMark     |✓|✓|  |  |✓|✓|  |  |  |  |  |  |
|SLAM              |✓|✓|  |  |  |  |  |  |✓|  |  |  |
|Face              |✓|✓|  |✓|✓|  |  |  |  |✓|  |  |


<a name="assets"></a>
# Assets

**ToF AR Samples Basic** provides the following assets. Assets include URP (Universal Render Pipeline) and VRM for 3D characters and avatars.

### TofArSamplesBasic
17 sample scene scripts and resources are stored for each component.

### TofArSettings
Prefabs and scripts are stored as the configuration change UI used by each component.


|File|Description|
|:--|:--|
|Settings.Prefab|Configuration change UI|
|XXXController.Prefab|Manage configuration changes for each component|

### URP（Universal Render Pipeline）

URP is the standard lightweight rendering pipeline of Unity for displaying scene content. URP is included in the asset.


Please see [Universal Render Pipeline documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/index.html).

### VRM
VRM is a platform-independent 3D avatar file format that handles humanoid character and avatars. VRM is included in the asset.

Please see [VRM documentation](https://vrm.dev/en/vrm/index).

<a name="environment"></a>
# Development environment

## Build library
ToF AR is required for build.
Please download the Toolkit from [the ToF AR Site on Developer World](https://developer.sony.com/develop/tof-ar), then import and use it.  
If the project is opened before importing, a confirmation message for entering safe mode will appear depending on the settings.
If safe mode is entered, please import after exiting safe mode from the safe mode menu etc.

## Documents

ToF AR Development documents are also available on Developer World.

* [ToF AR user manual](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_en.html) for overview and usage
* [ToF AR reference articles](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_Reference_Articles_en.html) for articles about each component
* [ToF AR API references](https://developer.sony.com/develop/tof-ar/development-guides/reference-api/reference/api/TofAr.V0.html)

## Verification environment

Operation was verified in the following environment:

* Unity Version  : 2020.3.36f1
* ToF AR Version : 1.1.0

<a name="contributing"></a>
# Contributing
**We cannot accept any Pull Request (PR) at this time.** However, you are always welcome to report bugs and request new features by creating issues.

We have released this program as a sample app with a goal of making ToF AR widely available. So please feel free to create issues for reporting bugs and requesting features, and we may update this program or add new features after getting feedback.
