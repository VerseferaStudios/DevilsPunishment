# DevilsPunishment
DevilsPunishment Repository

Note:
To use the UnityYAMLMerge.exe tool for merging .unity and .prefab files, add the [merge] section from the provided .gitconfig to your own ~/.gitconfig file

If you choose to do this, you should also install "Diff Merge" https://www.sourcegear.com/diffmerge/downloaded.php which will automatically be chosen as a "fallback merger" for files that UnityYAMLMerge cannot handle, by default.

There is also a way to choose any other fallback merge tool. You can read about it on the web related to issues with the UnityYAMLMerge.exe tool. The configuration for this (mergespecfile.txt) should be found in the same folder as the UnityYAMLMerge.exe.

I left out an important detail...
If you've configured "SmartMerge" as described above, when you merge you might see "CONFLICT"ed files still. If so then you need to use "smartmerge" on those conflicted files by using the command:
``` git mergetool ```
At this point git will attempt an auto merge using the merge tool that you've defined in your ~/.gitconfig file "Which should be SmartMerge, otherwise known as UnityYAMLMerge.exe"

After an automerge attempt, the system will ask you if the merge was succesful.
- If you choose the "y" (yes) option, git will assume everything went well.
- If you choose the "n" option, git will allow you to attempt the merge manually and will leave the conflicted file uncommitted.
- At this point some "temp" files may be generated. If you enter Unity to check whether the merge was successful you might generate other files, or even delete other files.
>- If this happens, make sure to "handle" the remaining uncommitted changes shown in "git status" (shown in red).
>>- My strategy is to "checkout" the files that unity deletes, and use ``` git clean -fd ``` to remove unessary generated temp files.
>>- After cleaning up this conflicted merge you should have only "green" files in your current "commit".
>>>- If the merge is completed "commit" your merge using ```git commit``` or ```git commit -m "[Message]"``` to commit your "merge"
If all is successful, I recommend "pushing your changes" to your "work branch" and continue developing happily.
