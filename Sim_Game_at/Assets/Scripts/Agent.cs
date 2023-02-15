using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Agent : MonoBehaviour
{

    public bool moving = false;
    public AgentData data;
    [SerializeField] string agentName;
    [SerializeField] string guid;
    private float speed = 1;
    // on the calc for the path nreverse the list

    private bool waiting = false;
    private float timeTaken = 3f;
    private Tile lastTile = null;


    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }



    public void LoadData(string guid)
    {
        data = GeneralUtil.dataBank.npcDict[guid];
        agentName = data.name;
        guid = data.guid;
    }

    public void SetPosition(Tile tilePos) => this.transform.position = new Vector3(tilePos.midCoord.x, 0.1f, tilePos.midCoord.z);



    //when we want the agent to go somewhere we set the movign thing to 
    private void Update()
    {
        if (data == null)
            return;

        if (data.pathTile.Count > 0 && data.moving == true) 
        {
            if (data.pathTile != null)
                PathFinding();
        }
        else
        {
            switch (data.currAction)
            {
                case AgentData.CURRENT_ACTION.WORKING: // is it working therefore is he on a entrance tile or is he on a resouce tile
                    StartCoroutine(AccessingResource());
                    break;
                case AgentData.CURRENT_ACTION.SLEEPING:     // just sleep and wait
                    break;
                case AgentData.CURRENT_ACTION.WONDERING:   // set a new path afyter a while
                    StartCoroutine(WonderingCall());
                    break;
                case AgentData.CURRENT_ACTION.RETURNING:   // dont know
                    break;
                case AgentData.CURRENT_ACTION.IDLE:   // state for the builing
                    break;
                case AgentData.CURRENT_ACTION.MOVING:   // dont know
                    break;
                default:
                    break;
            }

        }
    }

    public void PathFinding()
    {
        animator.SetBool("Walking", true);
        if (!GeneralUtil.AABBCol(this.transform.position, data.pathTile[0]))   // if it has yet to touch the tile
        {                                                                                                                                                     // tile modifier goes in here for the speed
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(data.pathTile[0].midCoord.x, 0.05f, data.pathTile[0].midCoord.z), speed * Time.deltaTime);


            transform.LookAt(new Vector3(data.pathTile[0].midCoord.x, 0.05f, data.pathTile[0].midCoord.z));
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);

        }
        else
        {
            lastTile = data.pathTile[0];
            data.pathTile.RemoveAt(0);

            if (data.pathTile.Count == 0)
                data.moving = false;

        }



        if (data.moving == false)
        {
            ReachedEntrance(lastTile);
        }

        //else
        //{


        //    animator.SetBool("Walking", false);
        //    waiting = true;


        //switch (data.currAction)
        //{
        //    case AgentData.CURRENT_ACTION.WORKING:
        //        StartCoroutine(AccessingResource());
        //        break;
        //    case AgentData.CURRENT_ACTION.SLEEPING:
        //        break;
        //    case AgentData.CURRENT_ACTION.WONDERING:
        //        StartCoroutine(WonderingCall());
        //        break;
        //    case AgentData.CURRENT_ACTION.RETURNING:
        //        break;
        //    case AgentData.CURRENT_ACTION.IDLE:
        //        break;
        //    case AgentData.CURRENT_ACTION.MOVING:
        //        break;
        //    default:
        //        break;
        //}
        //}


        //if (lastTile.tileType == TileType.ENTRANCE)
        //{
        //    Destroy(gameObject);
        //}

    }

    private void ReachedEntrance(Tile currTile)
    {
        Debug.Log(currTile.tileType);
        if (currTile.tileType == TileType.ENTRANCE)   //it reached work
        {
            Debug.Log("called");
            data.currAction = AgentData.CURRENT_ACTION.IDLE;
            Destroy(gameObject);
                //GeneralUtil.bank.ChangeFoodAmount(comp.foodAmount);
                //GeneralUtil.bank.ChangeStoneAmount(comp.stoneAmount);
                //GeneralUtil.bank.ChangeWoodAmount(comp.woodAmount);
            //give the resource to the building
        }
    }




    //wondering is for the hobos nothing to do in the day they just choose a ranomd place to go
    private IEnumerator WonderingCall()
    {
        yield return new WaitForSeconds(timeTaken * 2);

        var pos = new Vector2Int();

        for (int i = 0; i < GeneralUtil.map.tilesArray.Length; i++)
        {
            int row = i / GeneralUtil.map.textSize;
            int col = i % GeneralUtil.map.textSize;

            if (GeneralUtil.AABBCol(transform.position, GeneralUtil.map.tilesArray[row, col]))
            {
                pos = new Vector2Int(row, col);
                break;
            }
        }



        for (int i = 0; i < 5; i++)
        {
            var destination = new Vector2Int(pos.x + Random.Range(-10, 10), pos.y + Random.Range(-10, 10));
            if (destination.x < 0 || destination.y < 0 || destination.x >= GeneralUtil.map.tilesArray.GetLength(0) || destination.y >= GeneralUtil.map.tilesArray.GetLength(1))
            {
                continue;
            }

            var path = GeneralUtil.A_StarPathfinding(pos, destination, this.data);

            if (GeneralUtil.PathContainsTileType(TileType.WATER, path))
            {
                continue;
            }
            else
            {
                data.pathTile = path;

                waiting = false;
            }
        }
    }

    // this is for the workers
    private IEnumerator AccessingResource()
    {
        animator.SetBool("Working", true);

        yield return new WaitForSeconds(timeTaken);


        animator.SetBool("Working", false);

        Debug.Log(GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject);
        Debug.Log(lastTile.coord);

        if (GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject != null)
        {
            var obj = GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject;
            if (obj != null)
            {
                var comp = obj.GetComponent<Resource>();

                // this is where we add the resource to the char not the bank

                GeneralUtil.bank.ChangeFoodAmount(comp.foodAmount);
                GeneralUtil.bank.ChangeStoneAmount(comp.stoneAmount);
                GeneralUtil.bank.ChangeWoodAmount(comp.woodAmount);
            }
            waiting = false;



            GeneralUtil.map.tilesArray[lastTile.coord.x, lastTile.coord.y].tileObject = null;
            Destroy(obj);


        }

        data.SetAgentPathing(lastTile.coord, data.refToWorkPlace.entrancePoints[0], true);
       
    }



    private void OnDrawGizmosSelected()
    {

        if (data != null)
            return;

        if (data.pathTile.Count > 0)   // if the path is larger than 0 tiles
            return;

        foreach (var tile in data.pathTile)
            Gizmos.DrawSphere(tile.midCoord, 0.5f);

    }


}
