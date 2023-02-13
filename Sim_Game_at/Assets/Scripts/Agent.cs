using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    public bool moving = false;
    public NpcData data;
    [SerializeField] string agentName;
    [SerializeField] string guid;
    private float speed = 1;
    // on the calc for the path nreverse the list

    private bool doingJob = false;
    private float timeTaken = 3f;
    private Tile lastTile = null;


    private void Update()
    {
        PathFinding();

    }

    public void LoadData(string guid) 
    {
        data = GeneralUtil.dataBank.npcDict[guid];
        agentName = data.name;
        guid = data.guid;
    }

    public void SetPosition(Tile tilePos) => this.transform.position = new Vector3(tilePos.midCoord.x, 0.3f, tilePos.midCoord.z);
    public void PathFinding() 
    {
        if (data != null && !doingJob)
        {
            if (data.pathTile.Count > 0)   // if the path is larger than 0 tiles
            {
                if (!GeneralUtil.AABBCol(this.transform.position, data.pathTile[0]))   // if it has yet to touch the tile
                {                                                                                                                                                     // tile modifier goes in here for the speed
                    this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(data.pathTile[0].midCoord.x, 0.3f, data.pathTile[0].midCoord.z), speed * Time.deltaTime);
                }
                else
                {
                    lastTile = data.pathTile[0];
                    data.pathTile.RemoveAt(0);
                }
            }
            else 
            {
                doingJob = true; 
                if (lastTile.tileType == TileType.BLOCKED) 
                {
                    data.busy = false;
                    Destroy(gameObject);
                }
                else
                    StartCoroutine(DoingStuff());
            }
        }
    }





    private IEnumerator DoingStuff()
    {
        //age dependent
        yield return new WaitForSeconds(timeTaken);

        if (GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject != null)
        {
            var obj = GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject;
            if (obj != null)
            {
                var comp = obj.GetComponent<Resource>();
                GeneralUtil.bank.ChangeFoodAmount(comp.foodAmount);
                GeneralUtil.bank.ChangeStoneAmount(comp.stoneAmount);
                GeneralUtil.bank.ChangeWoodAmount(comp.woodAmount);
            }
            doingJob = false;



            GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject = null;
            Destroy(obj);


        }

        data.pathTile = GeneralUtil.A_StarPathfinding(new Vector2Int(lastTile.coord.x, lastTile.coord.y), new Vector2Int(data.refToWorkPlace.entrancePoints[0].x, data.refToWorkPlace.entrancePoints[0].y),this.data);
   
    }



    private void OnDrawGizmosSelected()
    {
        //for each tile draw something in the path
    }


}
