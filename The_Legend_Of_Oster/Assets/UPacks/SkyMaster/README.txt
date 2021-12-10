SETUP INSTRUCTIONS:
The system consists of a main script ("CloudHandlerSM_SRP.cs") which handles the
cloud shader variables for groups of clouds. The system requires a windzone to be
referenced in the "WindZone" slot and a Directional Light to be referenced as sun in 
the "Sun" slot of the script.

TUTORIAL VIDEO: https://www.youtube.com/watch?v=RB_q95eVjvE

CLOUD PLANES GROUP:
Each cloud group consists of a collection of planes or domes stacked one on top
of the other to emulate the volume effect. The effect can be used when above or
below the clouds. Multiple ready to use clouds are provided in sample scenes,
or the cloud shader "SkyMaster/ShaderVolumeClouds-SM3.0 v3.4." can be applied to any 
planar or dome mesh to create a layer, which can be copied and shifted vertically 
slightly to create a new group.


CLOUD DEFINITION IN THE SCRIPT:
The group of planes must be inserted in one of the slots for cloud groups 
("Flat Bed clouds" to "Rot Clouds"), each group can then be enabled or disabled by the
equivalent checkboxes ("Dome clouds" to "One Quad clouds") and set individual height, position
and sceling parameters (e.f. "Multi Quad cloud scale").


MATERIAL REFERENCING AND UPDATE:
Each group of cloud planes must have the same material and this material must be referenced in the
"Cloud Flat Mat" to "Cloud Rot Mat" slots, so the material properties are controled by the script.


SHADOWS:
For each group one shadow layer can be defined and must be referenced in the variables starting
from "Flat Bed S Plane" to "Rot Clouds S Plane". The shadow planes can be any mesh, using the same 
material as the clouds, with the difference that in the Mesh Renderer the "Cast Shadows" option must be 
set to "Shadows Only" choice.

The shadow object behaves like any other object doing shadows, so the effect is subject to the overall
Unity shadow setup and distances and will also interact with any effect that uses shadows like sun 
shafts.


CLOUD LIGHTING:
To globally control the cloud lighting use the "Intensity Diff", "Sun and Fog Offset" variables
for all cloud groups. To fine tune the lighting for each cloud group, the "_ColorDiffOffset" variable
on its material can be used, to further refine the specific coverage.


CLOUD COLORATION PER TIME OF DAY:
The clouds can be colored in various day section with smooth transition from each color to the next.
The Day Cloud color is the default main color (also Day cloud shadow color for under cloud color and Day 
cloud Fog color for distance fog color). The Dawn, Dusk and Night cloud color groups can be used to
define colors for the other time regions. To use those colors define a time in the "Current_Time"
variable (default is 12, which corresponds to Day group of colors). The system will lerp to the day 
section colors automatically after new time is defined. 

The "Coverage speed" variable defines how fast the transition between day section colors will happen.
The "Storm Sun Color" will be applied when weather is selected as storm.


ATMOSPHERIC SCATTERING EMULATION:
The system uses emulation of light scattering through the clouds, the "Fog_depth" to "K" parameters
control the scattering effect. "Mie Directional G" variable will focus the effect around the sun position,
use closer to one value to get most focus or closer to zero to get full spread. A middle value of 0.7 to
0.8 is recommended. "Lambda" and "K" variables control the coloration of the effect in the various scatter
regions and "Fog_depth" controls the overall intensity of the effect. Note that the directional light of
the scene must be referenced in the "Sun" slot of the clouds script in order for the scattering to be 
applied properly to the clouds.


CLOUD FOGGING:
There is three controls for the fogging of clouds, "Intensity fog offset" mentioned above, plus two
factors that affect the final color of the clouds, "Fog power" and "Fog power exp" that can fine tune
the effect to blend better with horizon or when volumetric fog from HDRP system is used.


CLOUD COVERAGE:
To globally control the cloud coverage and horizon distance use the "Coverage" and "Horizon" 
variables in the script. The "Cloud Density" value adjusts the overall scaling of the effect, use
higher values to emulate higher cloud bed with clouds looking higher away from the player.

To fine tune the coverage for each cloud group, the "_CoverageOffset" and 
"Thickness" variables on its material can be used, to further refine the specific coverage. The 
"_TransparencyOffset" variable on the material can be used to soften the edges of clouds on cloud
holes. 

If "Weather density" is defined, then the system can lerp between the chosen cloud presets defined
in "Cloud type" pull down menu. Clear day will apply the "Clear day coverage" and horizon variables
and disable lightning, Storm preset will apply storm cloud coverage and horizon and enable lightning.


CLOUD DENSITY CONTROL MAP:
A texture may also be used on the material to adjust the density, use the "_paintMap" slot to 
assign a texture and control the scaling and offset of its application on the clouds using the 
"Tiling" and "Offset" variables, next to the texture slot on the material.


CLOUD SPEED:
The clouds have main speed, controlled by "Wind strength" variable and the "Wind Parallax Factor"
dictates how the inner cloud motion behaves to give a parallax effect on top of the main motion.
The "Cloud Evolution" variable can also be used to fine tune the wind effect.


LIGHTNING:
The lightining system consists of a main script "ChainLightning_SKYMASTER" and a setup with Line Renderers
that render the effect on screen. The lightning uses the "Conductor" tag to define targets that 
lightning sources can strike towards. In the system there is also two point lights to emulate the 
lighting on ground and clouds. The clouds receieve one local light at each time which is passed by the 
cloud script to the shader to define the light position and intensity.

The system works by instantiating the lightning module in the voluem defined by the "Lightning Bounding Box" 
item referenced in the cloud script. Scale and translate this item to define where lightning 
will be instantiated. Use the "Lightning_every" variable to define how often strikes will happen and the
"Max_lightning_time" to define how fast strikes will be.

Use the "Enable Lightning" checkbox to enable lightning strikes. "Weather severity" also affects the rate
at which lightning will be enabled.


TOP DOWN VIEW USAGE:
To use the system when looking from above clouds or for ground fog, simply rotate the cloud plane group
tp look up (e.g. 180 degrees in Rotation of Z axis of the object holding the cloud planes).
Samples of this usuage are included in the demo scenes. Tweaking of the cloud heights may be required
to finalize the proper look for each case.


PREFABS SETUP:
The system can be setup fast using the various prefabs created from the demo scenes. Start by an empty
scene and insert the "HDRP BASE SETUP" prefab from the "VOLUMETRIC CLOUDS PREFABS" folder. This will add
a base HDRP setup and Directional Light. This step is not required as the camera, HDRP setup and light can
be defined by the user as needed or from existing scenes.

Then add any of the other prefabs to get the needed effect (e.g. Day or Dusk clouds, ground fog clouds,
top down view clouds etc). Optionally can also insert the "CLOUD REFLECTION" prefab to test clouds with
a reflection probe and get a sample setup for similar functionality. Note that this probe will reflect all
layers and cloud planes.

The "DAY TIME  CLOUDS OPTIMIZED with REFLECTION" prefab has both clouds and reflection setup included,
this is a sample of how to use Layers for some of the cloud planes to render a more performant reflection
of the clouds and is the recommended usage.

For "TOP DOWN VIEW CLOUDS" prefab move the camera to height of 1600 to see the effect.
For "FLOOR FOG" prefab move the camera to height of 300 to see the effect.
For "DUAL LAYER CLOUDS" and "DUAL LAYER CLOUDS DUSK" prefabs move the camera to height of 800 to see 
the effect.

IMPORTANT: For all prefabs, assign the Directional Light of the scene in the "Sun" slot of the clouds script
after insertion of the prefab in the scene.


SCRIPTING:
Scripting of the system can be done by changing the above mentioned parameters on script after referencing 
the cloud script. A sample "VolumeCloudsScriptingTemplate.cs" script is included to showcase some basic
scripting of the system.


BONUS MODULES:
The system has a few extra bonus modules, including a snow shader, a chain lightning usage (demo - prefabs
coming to next version), and a Volumetric Lit Particles system, that can be used to emulate clouds in
round planets or for lit smoke, to complement the main volume clouds effect.


SUPPORT:
For any help on using the system and for feedback and suggestions, please contact me in my Discord
channel (https://discord.gg/X6fX6J5) or my email: artengames@gmail.com


ROADMAP:
The following are planned for next versions
- Integration with Sky Master ULTIMATE for getting correct sun positioning based on time of day and world
position.
- Extra Weather presets to lerp cloud densities based on weather and addition of rain and snow effects
- Add 2ond point light capability (currently one point light can affect the clouds)
- Add Aurora effect
- Add refractive rain effect
- Add cloud density offset, to allow different scaling of clouds for different cloud groups
- Create variable sections for better script readability.

