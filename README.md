# DevilsPunishment
DevilsPunishment Repository

Note:
To use the UnityYAMLMerge.exe tool for merging .unity and .prefab files, add the [merge] section from the provide .gitconfig to your own ~/.gitconfig file

If you choose to do this, you should also install "Diff Merge" https://www.sourcegear.com/diffmerge/downloaded.php which will automatically be chosen as a "fallback merger" for files that UnityYAMLMerge cannot handle, by default.

There is also a way to choose any other fallback merge tool. You can read about it on the web related to issues with the UnityYAMLMerge.exe tool. The configuration for this (mergespecfile.txt) should be found in the same folder as the UnityYAMLMerge.exe.

I left out an important detail...
If you've configured "SmartMerge" as described above, when you merge you might see "CONFLICT"ed files still. If so then you need to use "smartmerge" on those conflicted files by using the command:
``` git mergetool ```
At this point git will attempt an auto merge using the merge tool that you've defined in your ~/.gitconfig file "Which should be SmartMerge, otherwise known as UnityYAMLMerge.exe"
