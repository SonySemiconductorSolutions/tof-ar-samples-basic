## 目次
* [ToF ARについて](#about)
* [ToF AR Samples Basicの概要](#overview)
* [コンポーネント](#component)
* [アセット](#assets)
* [開発環境](#environment)
* [コントリビューション](#contributing)

<a name="about"></a>
# ToF ARについて

ToF AR は、Time-of-Flight(ToF)センサを持つiOS/Andoroidスマートフォン向けの、Unity向けツールキットライブラリです。

Structured light 方式など、ToF 以外のDepthセンサでもToF ARは動作します。

サンプルアプリケーションのビルドと実行には、この ToF AR の他に、UnityとToFセンサを搭載した対応スマートフォンが必要です。

ToF AR のパッケージや開発ドキュメント、ToF ARを使ったアプリケーションソフト、対応スマートフォンのリストにつきましては、

Dveloper Wolrd の[ToF AR サイト](https://developer.sony.com/develop/tof-ar)をご覧ください。


<a name="overview"></a>
# ToF AR Samples Basicの概要

**ToF AR Samples Basic** は ToF AR の機能を使った簡単なサンプルアプリケーションで、下記の17シーンを提供しています。


## サンプルシーン一覧

<table>
<tr align="center">
    <th width="250">Color</th>
    <th width="250">ColorDepth</th>
    <th width="250">BasicStream</th>
    <th width="250">DepthConfidence</th>
</tr>
<tr align="center">
    <td>Colorイメージを表示</td>
    <td>ColorイメージとDepthイメージを重畳表示</td>
    <td>Depth/Confidence/Colorを並べて表示</td>
    <td>DepthイメージとConfidenceイメージを画面2分割で表示</td>
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
    <td>PointCloudを3次元空間に表示</td>
    <td>PointCloudを3次元空間にColor表示</td>
    <td>人物部分のPointCloudを3次元空間にColor表示</td>
    <td>人物、空のセグメンテーション結果をマスクとして表示</td>
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
    <td>手のボーンモデルを表示</td>
    <td>被写体のDepthに基づき、仮想キューブをオクルージョン表示</td>
    <td>手のDepthに基づき、仮想キューブをオクルージョン表示。あわせて手のボーンモデルを表示</td>
    <td>人体のボーンモデルを、Depthイメージの上に表示</td>
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
    <th width="250">Face</th>
</tr>
<tr align="center">
    <td>人体のボーンモデルを、DepthイメージとColorイメージを重畳した画像の上に表示</td>
    <td>人差し指で描いたマークを認識し、マーク名を表示</td>
    <td>人差し指で描いたマークを認識し、マーク名を表示。あわせてDepthイメージにColorイメージを重畳表示</td>
    <td>顔認識に基づき顔の上に白色マスクを表示。あわせて口の形からあいうえお認識結果を表示</td>
</tr>
<tr align="center">
    <td><img src="/Docs/images/13_ColorBody.jpg" width="150"></td>
    <td><img src="/Docs/images/14_HandMark.jpg" width="150"></td>
    <td><img src="/Docs/images/15_ColorHandMark.jpg" width="150"></td>
    <td><img src="/Docs/images/17_Face.jpg" width="150"></td>
</tr>
</table>

## 操作手順

ビルドしたアプリケーションの操作手順です。

1. アプリケーションを起動し、トップ画面のシーン一覧からシーンを選んでタップします。 

    <img src="/Docs/images/topmenu.jpg" width="150">

1. 画面左下のカメラアイコンをタップすると、選択したシーンで有効なカメラのリストが表示されます。カメラを選択すると、シーンの動作を開始します。　 

1. シーンからトップ画面に戻るには、4本の指で画面をタップします。


<a name="component"></a>
# コンポーネント

サンプルアプリケーションの17シーンと、各シーンが利用するToF AR コンポーネントの関係を示すテーブルです。縦にシーン名、横にコンポート名を並べています。チェックマークは、コンポーネント利用を示します。

||ToF|Color|Mesh|Coordinate|Hand|MarkRecog|Body|Segmentation|Face|Plane|Modeling|
|:--|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
|Color             |  |✓|  |  |  |  |  |  |  |  |  |
|ColorDepth        |✓|✓|  |✓|  |  |  |  |  |  |  |
|BasicStream       |✓|✓|  |  |  |  |  |  |  |  |  |
|DepthConfidence   |✓|  |  |  |  |  |  |  |  |  |  |
|PointCloud        |✓|  |  |  |  |  |  |  |  |  |  |
|ColorPointCloud   |✓|✓|  |✓|  |  |  |  |  |  |  |
|HumanPointCloud   |✓|✓|  |✓|  |  |  |✓|  |  |  |
|Segmentation      |  |✓|  |  |  |  |  |✓|  |  |  |
|Hand              |✓|✓|  |✓|✓|  |  |  |  |  |  |
|LiveMeshOcclusion |✓|✓|✓|✓|  |  |  |  |  |  |  |
|ColorHandOcclusion|✓|✓|✓|✓|✓|  |  |  |  |  |  |
|Body              |✓|  |  |  |  |  |✓|  |  |  |  |
|ColorBody         |✓|✓|  |✓|  |  |✓|  |  |  |  |
|HandHark          |✓|  |  |  |✓|✓|  |  |  |  |  |
|ColorHandMark     |✓|✓|  |  |✓|✓|  |  |  |  |  |
|Face              |✓|✓|  |✓|✓|  |  |  |✓|  |  |


<a name="assets"></a>
# アセット

**ToF AR Samples Basic**は、以下のアセットを提供します。アセットには、URP（Universal Render Pipeline）と、3DキャラクターやアバターのためのVRMも含まれます。

### TofArSamplesBasic
17のサンプルシーンのスクリプトやリソースが、コンポーネントごとに格納されています。

### TofArSettings
各コンポーネントが使用する設定変更UIとして、プレハブやスクリプトが格納されています。



|File|Description|
|:--|:--|
|Settings.Prefab|設定操作用UI|
|XXXController.Prefab|各コンポーネントの設定変更を管理|

### URP（Universal Render Pipeline）

シーンのコンテンツを画面に表示するための、Unity標準の軽量なレンダリングパイプラインです。アセットに含まれています。

詳細は[Universal Render Pipeline ドキュメント](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/index.html)を参照して下さい。

### VRM
人型のキャラクター・アバターを取り扱う、プラットフォーム非依存の3Dアバターファイルフォーマットです。アセットに含まれています。

詳細は[VRM ドキュメント](https://vrm.dev/en/vrm/index)を参照して下さい。

<a name="environment"></a>
# 開発環境

## ビルド用ライブラリ
ビルドには、ToF AR が必要です。
Developer Worldの[ToF AR サイト](https://developer.sony.com/develop/tof-ar)からダウンロードし、インポートして使用して下さい。

## ドキュメント

ToF ARの開発ドキュメントも、Developer Worldで公開しています。
* 概要や使い方についてのマニュアルは[ToF AR user manual](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_User_Manual_ja.html)
* 各コンポーネントの詳細記事は[ToF AR reference articles](https://developer.sony.com/develop/tof-ar/development-guides/docs/ToF_AR_Reference_Articles_ja.html)
* APIリファレンスは[ToF AR API references](https://developer.sony.com/develop/tof-ar/development-guides/reference-api-ja/reference/api/TofAr.V0.html)


## 動作検証環境

動作検証は、下記の環境で行っています。

* Unity Version  : 2020.3.28f1
* ToF AR Version : 1.0.0 

<a name="contributing"></a>
# コントリビューション
**現在、プルリクエストは受け付けておりません。** バグ報告や新規機能のリクエストがありましたらissueとして登録して下さい。

このサンプルプログラムはToF ARを広く利用して頂けるようリリースしております。ご報告頂いたissueについては、検討の上、更新で対応する可能性があります。
