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


    Vector3 destinationCoord = new Vector3(0, 0, 0);


    /// <summary>
    /// loads the data for this agent to use using the guid
    /// </summary>
    /// <param name="guid"></param>
    public void LoadData(string guid)
    {
        data = GeneralUtil.dataBank.npcDict[guid];
        agentName = data.name;
        guid = data.guid;
        data.agentObj = this.gameObject;
    }


    /// <summary>
    /// hard sets the current position of the selected agent
    /// </summary>
    /// <param name="tilePos"></param>
    public void SetPosition(Tile tilePos) => this.transform.position = new Vector3(tilePos.midCoord.x, 0.1f, tilePos.midCoord.z);









    private void Update()
    {
        if (data.pathTile.Count > 0) 
        {
            PathingCall();
        }
    }




    public void PathingCall() 
    {

        animator.SetBool("Walking", true);
        bool stillPathing = true;

        //if (!GeneralUtil.AABBCol(this.transform.position, data.pathTile[0]))   // if it has yet to touch the tile
        //{                                                                                                                                                     // tile modifier goes in here for the speed
        //    this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(data.pathTile[0].midCoord.x, 0.05f, data.pathTile[0].midCoord.z), speed * Time.deltaTime);

        //    transform.LookAt(new Vector3(data.pathTile[0].midCoord.x, 0.05f, data.pathTile[0].midCoord.z));
        //    transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
        //}


        if (!GeneralUtil.AABBCol(this.transform.position, data.pathTile[0]))   // if it has yet to touch the tile
        {                                                                                                                                                     // tile modifier goes in here for the speed
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(data.pathTile[0].midCoord.x + Random.Range(-0.4f, 0.4f), 0.05f, data.pathTile[0].midCoord.z + Random.Range(-0.4f, 0.4f)), speed * Time.deltaTime);

            transform.LookAt(new Vector3(data.pathTile[0].midCoord.x, 0.05f, data.pathTile[0].midCoord.z));
            transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
        }
        else
        {

            lastTile = data.pathTile[0];
            data.pathTile.RemoveAt(0);

            if (data.pathTile.Count == 0) 
            {
                stillPathing = false;
                if (GeneralUtil.entranceTileDict.ContainsKey(data.tileDestination)) 
                {
                    GeneralUtil.entranceTileDict[data.tileDestination].LandedOn(this.data);
                }
            }
        }


        if (!stillPathing)   //if he has done pathing
        {
            animator.SetBool("Walking", false);

            switch (data.currAction)
            {
                case AgentData.CURRENT_ACTION.WORKING:
                    if (data.tileDestination.tileType == TileType.ENTRANCE) //he is back at work
                    {
                        
                    }
                    else
                    {
                        StartCoroutine(MiningResource());
                    }

                    break;
                case AgentData.CURRENT_ACTION.SLEEPING:
                    //he is at home and has a home
                    break;
                case AgentData.CURRENT_ACTION.WONDERING:

                    //has no job
                    StartCoroutine(WonderingCall());

                    break;
                case AgentData.CURRENT_ACTION.HOMELESS:

                    //its night time and there is not home
                    break;
                default:
                    break;
            }







            //switch (data.currAction)
            //{
            //    case AgentData.CURRENT_ACTION.WORKING:

            //        if (data.tileDestination.tileType == TileType.ENTRANCE) //he is back at work
            //        {
            //            //data.readyToWork = true;
            //            //Destroy(gameObject);
            //        }
            //        else 
            //        {
            //            StartCoroutine(MiningResource());
            //        }

            //        break;
            //    case AgentData.CURRENT_ACTION.SLEEPING:
            //        break;
            //    case AgentData.CURRENT_ACTION.WONDERING:
            //        break;
            //    case AgentData.CURRENT_ACTION.RETURNING:
            //        break;
            //    case AgentData.CURRENT_ACTION.MOVING:
            //        break;
            //    case AgentData.CURRENT_ACTION.TRANSITION:

            //        data.currAction = data.hardSetAction;

            //        switch (data.currAction)
            //        {
            //            case AgentData.CURRENT_ACTION.WORKING:

            //                Debug.Log("is it getting here");
            //                data.readyToWork = true;
            //                Destroy(gameObject);
            //                break;
            //            case AgentData.CURRENT_ACTION.SLEEPING:
            //                break;
            //            case AgentData.CURRENT_ACTION.WONDERING:
            //                break;
            //            case AgentData.CURRENT_ACTION.RETURNING:
            //                break;
            //            case AgentData.CURRENT_ACTION.MOVING:
            //                break;
            //            case AgentData.CURRENT_ACTION.TRANSITION:
            //                break;
            //            default:
            //                break;
            //        }

            //        break;
            //    default:
            //        break;
            //}
        }
    }


   
    private IEnumerator WonderingCall()
    {
        yield return new WaitForSeconds(timeTaken * 2);


        for (int i = 0; i < 5; i++)
        {
            var path = GeneralUtil.A_StarPathfinding(lastTile.coord, new Vector2Int(lastTile.coord.x + Random.Range(-10, 10), lastTile.coord.y + Random.Range(-10, 10)), this.data, true);

            if (path != null) 
            {
                if (!GeneralUtil.PathContainsTileType(TileType.WATER, path))
                {
                    data.pathTile = path;
                }
            }
         
        }
    }

    // this is for the workers
    private IEnumerator MiningResource()
    {
        animator.SetBool("Working", true);

        yield return new WaitForSeconds(timeTaken);


        animator.SetBool("Working", false);


        if (GeneralUtil.map.tilesArray[data.tileDestination.coord.x, data.tileDestination.coord.y].tileObject != null)
        {
            var obj = GeneralUtil.map.tilesArray[data.tileDestination.coord.x, data.tileDestination.coord.y].tileObject;
            if (obj != null)
            {
                var comp = obj.GetComponent<Resource>();

                // this is where we add the resource to the char not the bank

                GeneralUtil.bank.ChangeFoodAmount(comp.foodAmount);
                GeneralUtil.bank.ChangeStoneAmount(comp.stoneAmount);
                GeneralUtil.bank.ChangeWoodAmount(comp.woodAmount);

            }

            Destroy(obj);
            GeneralUtil.map.tilesArray[data.tileDestination.coord.x, data.tileDestination.coord.y].tileObject = null;
        }

        data.SetAgentPathing(data.tileDestination.coord, data.refToWorkPlace.entrancePoints[0], true);

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
