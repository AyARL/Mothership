using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mothership;

/////////////////////////////////////////////////////////////////////////////
/// CLASS:               CNode
/////////////////////////////////////////////////////////////////////////////
class CNode
{
    // Depending on node type, the node controller will run different logic.
    public enum ENodeType
    {
        NODE_NONE,
        NODE_START,
        NODE_OPEN,
        NODE_CLOSED,
        NODE_END,
    }

    // Will hold the node position.
	private Vector3 m_v3Pos;
    public Vector3 NodePosition { get { return m_v3Pos; } }

    // Will hold the type of the node.
	private ENodeType m_eNodeType = ENodeType.NODE_NONE;
    public ENodeType NodeType { get { return m_eNodeType; } }

    // Will hold the weight of this node. This is used to calculate the fastest
    //  way through the nodes towards the target.
	private float m_fWeight = 0;
    public float Weight { get { return m_fWeight; } }

    // Will hold a reference to the previous node.
	private CNode m_cPrevNode;
    public CNode PreviousNode { get { return m_cPrevNode; } }
	
    // Will contain a list of all the nodes to which we have a clear line of sight.
	private List< CNode > m_liConnectedNodes = new List< CNode >();
    public List< CNode > ConnectedNodes { get { return m_liConnectedNodes; } }


	private List< CNode > m_liPotentialPrevPoints = new List< CNode >();
    public List<CNode> PotentialPrevNodes { get { return m_liPotentialPrevPoints; } }
		
    /////////////////////////////////////////////////////////////////////////////
    /// CTOR:               CNode
    /////////////////////////////////////////////////////////////////////////////
	public CNode( Vector3 v3Pos, ENodeType eType = ENodeType.NODE_NONE )
	{
        // Set up the node.
		m_v3Pos = v3Pos;
		m_eNodeType = eType;
	}
		
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               AddNode
    /////////////////////////////////////////////////////////////////////////////		
	public void AddNode( CNode cNode )
	{
        if ( null != cNode )
		    m_liConnectedNodes.Add( cNode );
	}
		
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               AddPrevNode
    /////////////////////////////////////////////////////////////////////////////
	public void AddPrevNode( CNode cNode )
	{
		m_liPotentialPrevPoints.Add( cNode );
	}
	
	/////////////////////////////////////////////////////////////////////////////
    /// Function:               SetPrevNode
    /////////////////////////////////////////////////////////////////////////////
	public void SetPrevNode(CNode cNode)
	{
		m_cPrevNode = cNode;
	}
		
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetState
    /////////////////////////////////////////////////////////////////////////////
	public void SetState( ENodeType eType )
	{
		m_eNodeType = eType;
	}
		
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               SetWeight
    /////////////////////////////////////////////////////////////////////////////
	public void SetWeight( float fWeight )
	{
		m_fWeight = fWeight;
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               AddPotentialPreviousNode
    /////////////////////////////////////////////////////////////////////////////
    public void AddPotentialPreviousNode( CNode cNode )
    {
        m_liPotentialPrevPoints.Add( cNode );
    }
}

/////////////////////////////////////////////////////////////////////////////
/// CLASS:               CNodeController
/////////////////////////////////////////////////////////////////////////////
public class CNodeController : MonoBehaviour {
	
    /////////////////////////////////////////////////////////////////////////////
    /// Function:               FindPath
    /////////////////////////////////////////////////////////////////////////////
	public static List< Vector3 > FindPath( Vector3 v3StartPos, Vector3 v3TargetPos )
	{
        // For error reporting.
        string strFunction = "CNodeController::FindPath()";

		// We need to check if we have a clear path towards the target.
		float fTargetDistance = Vector3.Distance( v3StartPos, v3TargetPos );

		if ( false == Physics.Raycast( v3StartPos, v3TargetPos - v3StartPos, fTargetDistance ) )
		{
            // There's nothing standing between the start position and the target,
            //  it's a straight forward path so we can set our NPC on his merry way.
			List< Vector3 > liPath = new List<Vector3>();

            // Add the start and target nodes to the path and return it.
			liPath.Add(v3StartPos);
			liPath.Add(v3TargetPos);

			return liPath;
		}
		
        // Retrieve all gameobjects with the node tag and create a node list using their positions.
		GameObject[] rggoNodes = GameObject.FindGameObjectsWithTag( Tags.TAG_NODE );
		List< CNode > liNodes = new List< CNode >();
		foreach ( GameObject goNode in rggoNodes )
		{
            // Create and add the node to the list.
			CNode cCurrNode = new CNode( goNode.transform.position );
			liNodes.Add( cCurrNode );
		}
		
        // Create a separate node for the start and target node.
        CNode cStartNode = new CNode( v3StartPos, CNode.ENodeType.NODE_START );
		CNode cTargetNode = new CNode( v3TargetPos, CNode.ENodeType.NODE_END );
		
        // Declare variables here for reuse inside the loops.
        float fDistance = 0;

		// Create the node connections for all our nodes.
		foreach( CNode cNode1 in liNodes )
		{
			foreach ( CNode cNode2 in liNodes )
			{
                // Check if this is the same node.
                if ( cNode1.NodePosition == cNode2.NodePosition )
                    continue;

                // Get the distance between the two nodes and check if there's anything standing between
				fDistance = Vector3.Distance( cNode1.NodePosition, cNode2.NodePosition );
				if ( false == Physics.Raycast( cNode1.NodePosition, cNode2.NodePosition - cNode1.NodePosition, fDistance ) )
				{
					Debug.DrawRay( cNode1.NodePosition, cNode2.NodePosition - cNode1.NodePosition, Color.white, 1);
					cNode1.AddNode( cNode2 );
				}
			}

            // Check if we can get to the target node.
			fDistance = Vector3.Distance( v3TargetPos, cNode1.NodePosition );
			if ( false == Physics.Raycast( v3TargetPos, cNode1.NodePosition - v3TargetPos, fDistance ) )
			{
				Debug.DrawRay( v3TargetPos, cNode1.NodePosition - v3TargetPos, Color.white, 1 );
				cNode1.AddNode( cTargetNode );
			}
		}
		
		// Loop through a the node list and find all nodes which the NPC can use to travel to from the start node.
		foreach ( CNode cNode in liNodes )
		{
            // Get the distance between the current node and the start position and cast a ray to see if we
            //  have any obstacles in the way.
			fDistance = Vector3.Distance( v3StartPos, cNode.NodePosition );
			if (false == Physics.Raycast( v3StartPos, cNode.NodePosition - v3StartPos, fDistance ) )
			{
				// There's nothing in the way, we can travel to this node if we choose to.
                Debug.DrawRay( v3StartPos, cNode.NodePosition - v3StartPos, Color.white, 1);
				
                // Set the previous node to the start node.
				cNode.SetPrevNode( cStartNode );

                // Indicate that this node is open and we can use it.
				cNode.SetState( CNode.ENodeType.NODE_OPEN );

                // Use the total distance from the start position to the end node as weight.
				cNode.SetWeight( fDistance + Vector3.Distance( v3TargetPos, cNode.NodePosition ) );
			}
		}
		
		bool bSearchedAll = false;
		bool bFoundEnd = false;
		
		while( false == bSearchedAll )
		{
            // Searched for all flag will be true unless there are open nodes in the nodelist.
			bSearchedAll = true;

            // Will hold a list of potential path members.
            List< CNode > liOfInterest = new List< CNode >();

			foreach ( CNode cNode in liNodes )
			{
				if ( cNode.NodeType == CNode.ENodeType.NODE_OPEN )
				{
                    // Indicate that there are still open nodes
					bSearchedAll = false;

                    // Get a list of all connected nodes.
					List< CNode > liConnectedNodes = cNode.ConnectedNodes;
					foreach ( CNode cPotentialNode in liConnectedNodes )
					{
                        switch ( cPotentialNode.NodeType )
                        {
                            case CNode.ENodeType.NODE_NONE:

                                // Add our current node as the previous node of the potential node.
                                cPotentialNode.AddPotentialPreviousNode( cNode );

                                // Add a potential path waypoint.
                                liOfInterest.Add( cPotentialNode );

                                // Set the node weight.
							    cPotentialNode.SetWeight( Vector3.Distance( v3StartPos, cPotentialNode.NodePosition ) + Vector3.Distance( v3TargetPos, cPotentialNode.NodePosition ) );

                                break;

                            case CNode.ENodeType.NODE_END:

                                // We found the exit.
							    bFoundEnd = true;
							    cTargetNode.AddNode( cNode );

                                break;
                        }
					}

                    // Set this node to closed so we don't reuse it.
					cNode.SetState( CNode.ENodeType.NODE_CLOSED );
				}
			}

            // Set the previous points for the potential path members.
            PopulatePrevNodes( ref liOfInterest );
		}
		
        // Check if we found the end node.
		if ( bFoundEnd )
		{
            // This list will hold the shortest path to the target.
            List< CNode > liShortestPath = null;

            // Will hold the lowest weight.
			float fLowestWeight = -1;

			// Trace back finding the route with the least weight.
			foreach ( CNode cNode in cTargetNode.ConnectedNodes )
			{
                // We're holding a separate reference to cNode so we can trace back
                //  without modifying cNode's value.
                CNode cCurrentNode = cNode;
				float fWeight = 0;
				bool bTracing = true;
				
                // Will hold the full path after we're done tracing.
				List< CNode > liPath = new List< CNode >();
				liPath.Add( cTargetNode );

				while( bTracing )
				{
					liPath.Add( cCurrentNode );
					if ( cCurrentNode.NodeType == CNode.ENodeType.NODE_START )
					{
                        // Lowest weight hasn't been set yet, set it now.
						if ( -1 == fLowestWeight )
						{
							liShortestPath = liPath;
							fLowestWeight = fWeight;
						} 

                        else
						{
                            // Check if we found a faster path.
							if ( fLowestWeight > fWeight )
							{
								liShortestPath = liPath;
								fLowestWeight = fWeight;
							}
						}
						bTracing = false;
						break;
					}

                    // Add the current node's weight to the total weight of this path
                    //  and find the next previous node.
					fWeight += cCurrentNode.Weight;
					cCurrentNode = cCurrentNode.PreviousNode;
				}
			}
			
            // Reverse the path and return a list of vectors to the caller.
			liShortestPath.Reverse();
			List< Vector3 > liWantedPath = new List< Vector3 >();
			foreach ( CNode cNode in liShortestPath )
			{
				liWantedPath.Add( cNode.NodePosition );
			}
			return liWantedPath;
		}
        else
		{
            // We did not manage to find a path, report the issue and return null
            Debug.LogError( string.Format( "{0} {1}", strFunction, ErrorStrings.ERROR_PATHFINDING_NO_VALID_PATH ) );
			return null;
		}
	}

    /////////////////////////////////////////////////////////////////////////////
    /// Function:               PopulatePrevNodes
    /////////////////////////////////////////////////////////////////////////////
    private static void PopulatePrevNodes( ref List< CNode > liOfInterest )
    {
        // Loop through the list of interest and attempt to find the previous node
        //  with the least weight.
        foreach ( CNode cNode in liOfInterest )
        { 
            // Set the node to open.
            cNode.SetState( CNode.ENodeType.NODE_OPEN );

            // Will hold a reference to the preferred previous node which we're going
            //  to return.
            CNode cReturnNode = null;

            // Keep track of the lowest weight.
            float fLowestWeight = -1;

            // Loop through the list of potential previous nodes.
            foreach ( CNode cPrevNode in cNode.PotentialPrevNodes )
            {
                // Check if the lowest weight has been initialized ( -1 == false )
                if ( -1 == fLowestWeight )
                {
                    fLowestWeight = cPrevNode.Weight;
                    cReturnNode = cPrevNode;
                    continue;
                }
                else
                {
                    if ( fLowestWeight > cPrevNode.Weight )
                    {
                        fLowestWeight = cPrevNode.Weight;
                        cReturnNode = cPrevNode;
                    }
                }
            }

            // Set the preferred prev node.
            cNode.SetPrevNode( cReturnNode );
        }
    }
}