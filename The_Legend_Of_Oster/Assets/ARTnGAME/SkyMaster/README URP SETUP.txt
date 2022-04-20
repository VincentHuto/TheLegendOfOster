URP SETUP INSTRUCTIONS:

The system has 3 Native URP modules, the Volumetric clouds with 3D Noise ("InfiniCLOUD_URP" folder), the volumetric fog ("VolumeFogSRP" folder) 
and the sun shafts ("SunShaftsSRP" folder). 

The "InfiniCLOUD" folder contains the Volumetric clouds with 2D Noise system, that is implemented
as a dome with the volumetric shader, thus is not using the native custom renderer feature to render the clouds and can be used with the 3D noise
clouds or standalone as needed. Since the cloud dome is an object in the scene and not image effect, will appear in reflections by default.

To use the URP Native modules:

1. Add the sample "UniversalRP-HighQuality" pipeline from "URP Custom Renderer" -> "Settings", that implements
the sample "ForwardRenderer" custom renderer. This renderer must have the "New Blit Volume Fog SRP", New BlitSun Shafts SRP" and
"New Blit Volume Clouds SRP" renderer feature scripts (if all three are required). Please upgarde the pipeline to 7.3.1 version 
before importing the package and this sample pipeline, so it can function and import properly.

2. Add to the camera the "ConnectSunToSunShaftsURP",  "ConnectSunToVolumeFogURP" and  "ConnectSunToVolumeCloudsURP" scripts (or the
relevant ones based on which renderer features are in the forward renderer, if choose to use only few of the three),
and regulate the effects as needed, demo scenes with sample settings are provided for reference. Note that those scripts
must be always on camera and enabled, even when a scene does not need the specific effect, so they can control the active renderer features. 
To disable the effect uncheck the "Enable Shafts" for sun shafts or the "Enable Fog" checkboxes for Clouds and Fog, with the scripts enabled.

3. Place the scene directional light in the "Sun" slot of the scripts. This is required for the system to get the sun light information.
The "Sun" must be defined even when the effect is not used (checkbox disabled as described in above section).

SUPPORT:
For any help on using the system and for feedback and suggestions, please contact me in my Discord
channel (https://discord.gg/X6fX6J5) or my email: artengames@gmail.com


ROADMAP:
The following are planned for next versions
- Integration with Sky Master ULTIMATE for getting correct sun positioning based on time of day and world
position and create a complete Sky Master URP package.
- Implement few final features from Standard Pipeline that are missing in URP.
- Start renderer features from a disabled state, so there will be no need to have all the control scripts on the Main camera
if not used in the scene.

