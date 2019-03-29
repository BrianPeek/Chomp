mkdir sheets
del /q sheets\*.*
montage gfx\monster\walk\bm_walk_r_%%d.png[1-16] -geometry +0+0 -tile 8x2 -background none sheets\bm_walk.png
montage gfx\monster\idle\bm_idle_%%d.png[1-16] -geometry +0+0 -tile 8x2 -background none sheets\bm_idle.png
montage gfx\enemy\walk\rm_walk_r_%%d.png[1-16] -geometry +0+0 -tile 8x2 -background none sheets\rm_walk.png
montage gfx\enemy\idle\rm_idle_%%d.png[1-16] -geometry +0+0 -tile 8x2 -background none sheets\rm_idle.png

montage sheets\*.png -geometry +0+0 -tile 1x4 -background none sheets\monsters.png
convert sheets\monsters.png -gravity northwest -background none -extent 1024x1024 sheets\ChompMonsters.png

montage gfx\dot\dot%%d.png[1-5] -geometry +0+0 -tile 5x1 -background none sheets\dot.png
convert sheets\dot.png -gravity northwest -background none -extent 256x256 sheets\dot_256.png
