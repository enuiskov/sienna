using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using AE.Editor;
using AE.Visualization;
//using AE.Data.DescriptionLanguage;
//using AE.Data.DescriptionLanguage.Scripting;

namespace AE.Visualization.SchemeObjectModel
{
	public enum EdgeType
	{
	    Blind, //NoWay
	    //Copy,  //explicit 

	    ForwardPropagation,  //explicit 
	    BackwardPropagation,  //explicit 



	    Conditional,//
	    Sequence,

	    /**
	        0000 - last node (no escape)
	        0E00 - intermediate node
	        0EE0 - multiple escapes (forces parallel execution)
	        0C00 - 
	        0000 - 
	        0000 - 
	        0000 - 
	        0SSS - Sequence (clockwise?)
	    */
	}
	public class NGonEdgeInfo
	{
		public int      Index;
		public EdgeType Type;
		public Vector3d Vertex1;
		public Vector3d Vertex2;

		public NGonNode[] Neighbours;
	}
	public partial class NGonScriptNode : NGonNode
	{
		///public Program Script;

		public void ProcessScript()
		{
			//this.Ports.
			foreach(var cPort in this.Ports)
			{
				//cPort.Type
			}
			//this.Ports[0
		}

			//public NGonNode[]                   Neighbours;
		//public NeighbourEnvironment     Neighbours;
		//public NeighbourGroup               Surroundings;

		//public NGonNode[]               Neighbours;

		//public int                     TargetCount;
		//public bool[]                  Targets;
		//public bool                    IsProcessing {get{return this.NodeState == NodeState.Executing;}}
		//public NodeState               NodeState{get;set;}

		//public float                   Value;
		//public float[]                 Inputs;
		//public float[]                 Outputs;
		//public TransmissionPort[]      Inputs;
		//public TransmissionPort[]      Outputs;
		


		//public void ProcessInputs()
		//{
		//    //for(var 
			
		//}
		//public void ProcessTransmission()
		//{
		//    var _Value = this.Value;

		//    //foreach(var cNeighNode in this.Neighbours)
		//    for(var cSi = 0; cSi < this.Sides; cSi++)
		//    {
		//        var cOutput = Math.Min((0f + _Value) * this.Outputs[cSi], 1f);


		//        var cNeighNode = this.Neighbours[cSi]; if(cNeighNode == null) continue;

		//        var cNeighsSide = cNeighNode.GetNeighbourSide(this);
		//        cNeighNode.Inputs[cNeighsSide] += cOutput;
		//    }
		//}
		//public void UpdateValue()
		//{
		//    //var _NewValue = this.Value * 0.9f;

		//    var _ValueSum = 0f;
		//    {
		//        for(var cSi = 0; cSi < this.Sides; cSi++)
		//        {
		//            _ValueSum += this.Inputs[cSi];
		//        }
		//    }
		//    var _NewValue = this.Value + _ValueSum;// / this.Sides;

		//    this.Value = MathEx.Clamp(_NewValue, 0f, 1.0f);





		//    for(var cSi = 0; cSi < this.Sides; cSi++)
		//    {
		//        this.Outputs[cSi] = this.Value * 1f;
		//    }
		//}
		
		//public List<NGonNode> GetTargetNodes(EdgeType iPropMode)
		//{
		//    //this.SrcEdges;
		//    //this.SourceNodes
		//    //var _SrcNode this.Neighbours[cTi]

		//    //float _SigSum = 0f;
		//    //{
		//    //    for(var cSi = 0; cSi < this.Sides; cSi++)
		//    //    {
		//    //        _SigSum += this.SrcEdges[cSi];
		//    //    }
		//    //}
		//    var oTgtNodes = new List<NGonNode>(this.Sides);
		//    {
		//        for(var cSi = 0; cSi < this.Sides; cSi++)
		//        {
		//            var cNeighGroup = this.Neighbours[cSi];
					
		//            if(cNeighGroup != null && this.EdgeTypes[cSi] == iPropMode)
		//            {
		//                oTgtNodes.AddRange(cNeighGroup.Nodes);
		//            }
		//        }
		//    }
		//    return oTgtNodes;
		//}
		//public          void UpdateNodeLinks     (bool iRecursive)
		//{
		//    return;

		//    //var _OldNeighbours = this.Neighbours;
			
		//    //this.Neighbours = new NGonNode[this.Sides];

		//    //if(this.Parent != null)
		//    //{
		//    //    foreach(var caPoint in this.ContactPoints)
		//    //    {
		//    //        foreach(NGonNode cSiblingNode in this.Parent.Children)
		//    //        {
		//    //            if(cSiblingNode == this) continue;
		//    //            ///if((cSiblingNode.Position - this.Position).Length > 10) continue;

		//    //            foreach(var cbPoint in cSiblingNode.ContactPoints)
		//    //            {
		//    //                var cabDist = (caPoint.Position - cbPoint.Position).Length;

		//    //                if(cabDist < 0.1) //~~ threshold
		//    //                {
		//    //                    caPoint.Nodes.Add(cSiblingNode);
		//    //                    break;
		//    //                }
		//    //            }
		//    //        }
		//    //    }
		//    //    //if(this.IsActiveNode)
		//    //    //{
					
		//    //    //}
		//    //    for(int _LastPi = this.Sides - 1, cSi = 0; cSi < this.Sides; cSi++)
		//    //    {
		//    //        var cPi = cSi;//(cSi == _LastPi ? 0 : cSi);
		//    //        var nPi = cSi == _LastPi ? 0 : cSi + 1;

		//    //        var cPoint = this.ContactPoints[cPi];
		//    //        var nPoint = this.ContactPoints[nPi];

		//    //        var cPointOO = cPoint.Nodes;
		//    //        var nPointOO = nPoint.Nodes;


		//    //        for(var cxOi = 0; cxOi < cPointOO.Count; cxOi++)
		//    //        {
		//    //            for(var cyOi = 0; cyOi < nPointOO.Count; cyOi++)
		//    //            {
		//    //                if(cPointOO[cxOi] == nPointOO[cyOi])
		//    //                {
		//    //                    this.Neighbours[cPi] = cPointOO[cxOi] as NGonNode;
		//    //                }
		//    //            }
		//    //        }
		//    //        //for(var cObj in cPoint.Nodes)
		//    //    }
		//    //}
		//    ////if(this.IsActiveNode)
		//    ////{
				
		//    ////}

		//    //if(_OldNeighbours != null)
		//    //{
		//    //    for(var cNi = 0; cNi < this.Neighbours.Length; cNi++)
		//    //    {
		//    //        var cOldN = _OldNeighbours[cNi];
		//    //        var cNewN = this.Neighbours[cNi];

		//    //        if(cNewN != cOldN)
		//    //        {
		//    //            if(cOldN != null) {cOldN.UpdateContactPoints(false); cOldN.UpdateNodeLinks(false);}
		//    //            if(cNewN != null) {cNewN.UpdateContactPoints(false); cNewN.UpdateNodeLinks(false);}
						
		//    //        }
		//    //    }


		//    //    //foreach(NGonNode cNode in _OldNeighbours)
		//    //    //{
		//    //    //    if(cNode == null) continue;

		//    //    //    for(var cNi = 0; cNi < cNode.Neighbours.Length; cNi++)
		//    //    //    {
		//    //    //        if(cNode.Neighbours[cNi] == this)
		//    //    //        {
		//    //    //            cNode.Neighbours[cNi] = null;
		//    //    //        }
		//    //    //    }
		//    //    //}
		//    //}


		//    //if(iRecursive) foreach(NGonNode cNode in this.Children)
		//    //{
		//    //    cNode.UpdateNodeLinks(iRecursive);
		//    //}

		//}
		//public          int  GetNeighbourSide    (NGonNode iNeighNode)
		//{
		//    return 0;
		//    //for(var cSi = 0; cSi < this.Sides; cSi++)
		//    //{
		//    //    var cNeighNode = this.Neighbours[cSi];

		//    //    if(cNeighNode == iNeighNode) return cSi;

		//    //}
		//    //throw new WTFE();
		//}
		

	}
}
