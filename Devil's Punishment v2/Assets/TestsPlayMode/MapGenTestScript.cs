using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class MapGenTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void MapGenTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator VentTagFloor_WithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            SceneManager.LoadScene("MapGen 4");

            WaitForSeconds yield5Sec = new WaitForSeconds(5f);

            MapGen3 mapGen3 = null;
            for (int i = 0; i < 5; i++)
            {
                yield return yield5Sec;
                mapGen3 = Object.FindObjectOfType<MapGen3>();
            }

            Assert.NotNull(mapGen3);

            mapGen3.startMapGeneration(135);

            yield return new WaitForSeconds(10f);

            RoomNew roomNew = Object.FindObjectOfType<RoomNew>();

            Assert.NotNull(roomNew);

            yield return new WaitUntil(() => roomNew.isDoneSpawningAllCorridorsOrVents);

            InteractableDoor[] interactableDoors = Object.FindObjectsOfType<InteractableDoor>();
            for (int i = 0; i < interactableDoors.Length; i++)
            {
                Transform ventCover = interactableDoors[i].transform.parent?.parent;
                if(ventCover.name.Equals("Vent Cover"))
                {
                    Transform spawnPoint = ventCover.GetChild(1);
                    if(spawnPoint.position.y > 20)
                    {
                        //second floor
                        Assert.AreEqual(spawnPoint.tag, "Vent Spawn Points 2nd Floor");
                    }
                    else
                    {
                        Assert.AreEqual(spawnPoint.tag, "Vent Spawn Points");
                    }
                }
                else
                {
                    //might be door and not vent cover
                    continue;
                }
            }

            yield return null;
        }

        [TearDown]
        public void AfterEveryTest()
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        [UnityTest]
        public IEnumerator ElevatorOverlapFloor2_WithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            SceneManager.LoadScene("MapGen 4");

            WaitForSeconds yield5Sec = new WaitForSeconds(5f);

            MapGen3 mapGen3 = null;
            MapGen2ndFloor mapGen2ndFloor = null;
            for (int i = 0; i < 5; i++)
            {
                yield return yield5Sec;
                mapGen3 = Object.FindObjectOfType<MapGen3>();
            }

            Assert.NotNull(mapGen3);
            mapGen2ndFloor = Object.FindObjectOfType<MapGen2ndFloor>();
            Assert.NotNull(mapGen2ndFloor);

            mapGen3.isDevMode = true;
            mapGen2ndFloor.isDevMode = true;

            mapGen3.startMapGeneration(135);

            yield return new WaitForSeconds(10f);

            //yield return new WaitUntil(() => roomNew.isDoneSpawningAllCorridorsOrVents);

            //lift room stuff
            Assert.AreNotEqual(mapGen3.allRooms[0], -Data2ndFloor.instance.liftRoomPos);
            Assert.AreEqual(mapGen3.allRooms[1], mapGen2ndFloor.allRooms[0]);

            //room count and position check
            GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
            //count
            Assert.AreEqual(rooms.Length, mapGen3.numberOfRooms + mapGen2ndFloor.numberOfRooms - 1);// extra -1 for elevator in floor 2
            for (int i = 0; i < rooms.Length; i++)
            {
                //pos
                Vector3 room1Pos = rooms[i].transform.position;
                for (int j = i + 1; j < rooms.Length; j++)
                {
                    if (IsNoRoomCollision(room1Pos, rooms[j].transform.position, mapGen3 /*later choose correctly according to fllor maybe*/))
                    {
                        continue;
                    }
                    else
                    {
                        Assert.Fail("rooms are colliding " + room1Pos + " and " + rooms[j].transform.position, room1Pos);
                    }
                }
            }


            //maybe find a stronger way to test elevator stuff


            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos1"> position of room 1 </param>
        /// <param name="pos2"> position of room 2 </param>
        /// <param name="mapGenBase"></param>
        /// <returns> returns true if there is no room collision between the given rooms </returns>
        private bool IsNoRoomCollision(Vector3 pos1, Vector3 pos2, MapGenBase mapGenBase)
        {
            if (Mathf.Abs(pos1.y - pos2.y) > 7.5f)
            {
                return true;
            }
            else
            {
                return Mathf.Abs(pos1.x - pos2.x) >= mapGenBase.xSize ||
                       Mathf.Abs(pos1.z - pos2.z) >= mapGenBase.zSize;
            }
        }

    }
}
