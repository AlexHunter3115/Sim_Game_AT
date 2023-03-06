using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public bool moving = false;
    public AgentData data;
    [SerializeField] string agentName;
    [SerializeField] string guid;
    private float speed = 1;
    // on the calc for the path nreverse the list

    private float timeTaken = 3f;
    private Tile lastTile = null;

    public bool switchPathMode = false;
    public bool showPathToggle = false;

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
        animator.SetBool("Working", false);
        bool stillPathing = true;

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
        //they shouldnt cross the river and they should keep the anim

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

        }
    }
   
    private IEnumerator WonderingCall()
    {
        yield return new WaitForSeconds(timeTaken * Random.Range(1.0f,2.0f));
        //the issue is the courutine still will call even if the wait time is gone so this is the check for that
        if (data.currAction == AgentData.CURRENT_ACTION.WONDERING)
            data.SetAgentPathing(GeneralUtil.WorldPosToTile(this.transform.position).coord, GeneralUtil.RandomTileAround(4, GeneralUtil.WorldPosToTile(this.transform.position).coord, new List<int> { 0, 1 }, 10).coord, true);
    }

    // this is for the workers
    private IEnumerator MiningResource()
    {
        animator.SetBool("Working", true);

        yield return new WaitForSeconds(timeTaken);

        animator.SetBool("Working", false);

        if (data.currAction == AgentData.CURRENT_ACTION.WORKING) 
        {
            if (GeneralUtil.map.tilesArray[data.tileDestination.coord.x, data.tileDestination.coord.y].tileObject != null)
            {
                var obj = GeneralUtil.map.tilesArray[data.tileDestination.coord.x, data.tileDestination.coord.y].tileObject;
                if (obj != null)
                {
                    var comp = obj.GetComponent<Resource>();

                    // this is where we add the resource to the char not the bank

                    GeneralUtil.resourceBank.ChangeFoodAmount(comp.foodAmount);
                    GeneralUtil.resourceBank.ChangeStoneAmount(comp.stoneAmount);
                    GeneralUtil.resourceBank.ChangeWoodAmount(comp.woodAmount);

                    switch (comp.type)
                    {
                        case Resource.RESOURCE_TYPE.STONE:
                            GeneralUtil.dataBank.numOfResourcesStone--;
                            break;
                        case Resource.RESOURCE_TYPE.FOOD:
                            GeneralUtil.dataBank.numOfResourcesFood--;
                            break;
                        case Resource.RESOURCE_TYPE.WOOD:
                            GeneralUtil.dataBank.numOfResourcesWood--;
                            break;
                        default:
                            break;
                    }
                }

                Destroy(obj);
                GeneralUtil.map.tilesArray[data.tileDestination.coord.x, data.tileDestination.coord.y].tileObject = null;
            }

            data.SetAgentPathing(data.tileDestination.coord, data.refToWorkPlace.entrancePoints[0], true);
        }

    }


    //this needs ot change anwyay to a norm functiona s gizmos draw on the eidtor
    private void OnDrawGizmos()
    {
        if (data == null)
            return;

        if (data.pathTile.Count == 0)   // if the path is larger than 0 tiles
            return;


        if (showPathToggle)
        {
            if (switchPathMode)
            {
                Gizmos.color = Color.red;

                foreach (var tile in data.pathTile)
                {
                    Gizmos.DrawSphere(tile.midCoord, 0.25f);
                }
            }
            else
            {
                Gizmos.color = Color.yellow;

                foreach (var tile in data.allCheckedDebug)
                {
                    Gizmos.DrawSphere(tile.midCoord, 0.25f);
                }
            }
        }
    }

}
