using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace AE
{
	public class DataNode : IEnumerable<DataNode>, IComparable<DataNode>, ICloneable
	{
		public class	Collection	: IEnumerable<DataNode>, IEnumerable			
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public DataNode			Parent;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public XmlNodeType		ItemsType;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public List<DataNode>	Items;
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public int				Count													
			{
				get
				{
					return this.Items.Count;
				}
			}

			public Collection(DataNode iParent, XmlNodeType iItemsType)						
			{
				this.Parent    = iParent;
				this.ItemsType = iItemsType;

				this.Items = new List<DataNode>();
				{
					if(iItemsType == XmlNodeType.Attribute)
					{
						foreach(XmlAttribute cAttribute in iParent.XmlNode.Attributes)
						{
							this.Items.Add(new DataNode(cAttribute));
						}
					}
					else
					{
						foreach(XmlNode cChildNode in iParent.XmlNode.ChildNodes)
						{
							this.Items.Add(new DataNode(cChildNode));
						}
					}
				}
			}

			public DataNode this	[string iNodeID, params string[] iNsDefinitions]		
			{
				get
				{
					return Routines.GetNodeByExpression(iNodeID, this.Parent, iNsDefinitions);
				}
				set
				{
					Routines.SetNodeByID(value, iNodeID, this);
				}
			}
			public DataNode this	[int iIndex]											
			{
				get
				{
					return this.Items[iIndex];
				}
			}

			public void Clear()
			{
				if(this.ItemsType == XmlNodeType.Attribute)
				{
					this.Parent.XmlNode.Attributes.RemoveAll();
				}
				else if(this.ItemsType == XmlNodeType.Element)
				{
					XmlNodeList _ChildNodes = this.Parent.XmlNode.ChildNodes;
					{
						while(this.Parent.XmlNode.ChildNodes.Count != 0)
						{
							this.Parent.XmlNode.RemoveChild(this.Parent.XmlNode.ChildNodes[0]);
						}
					}
					
				}
			}

			public override string	ToString()												
			{
				return "Count: " + this.Count;
			}

			#region Interfaces
			public IEnumerator<DataNode> GetEnumerator()
			{
				List<DataNode> _NodeList = new List<DataNode>();
				{
					foreach(DataNode cNode in this.Items)
					{
						switch(cNode.Type)
						{
							case XmlNodeType.Comment : continue;

							default:
							{
								_NodeList.Add(cNode);
								continue;
							}
						}
					}
				}
				return _NodeList.GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}
		public struct	Routines													
		{
			//public class Standard						
			//{
				public static readonly string	DefaultNodeName		= "DataNode.Null";

				
				public static readonly string	IndexOpener			= "[";
				public static readonly string	IndexCloser			= "]";

				public static readonly char		LevelDelimiter		= '/';
				public static readonly char		AttributeSign		= '@';
				public static readonly char		NodeNameDelimiter	= ':';
			//}
			//public class Checks							
			//{
				public static bool			IsAttributePath			(string iNodePath)		
				{
					return iNodePath.Contains(AttributeSign.ToString());
				}
				public static bool			IsAttributeID			(string iNodeID)			
				{
					return iNodeID.StartsWith(AttributeSign.ToString());
				}
			//}
			//public class PathWorks						
			//{
				public static string[]		GetLevelsFromPath		(string iPath)			
				{
					return iPath.Trim(LevelDelimiter).Split(LevelDelimiter);
				}
				public static string		GetPathFromLevels		(string[] iLevels)		
				{
					///Res: "html/head/title" (no start slash)
					return String.Join(LevelDelimiter.ToString(), iLevels);
				}

				public static string		GetHighestLevel			(string iPath)			
				{
					string[] _Levels = GetLevelsFromPath(iPath);
					{
						if(_Levels.Length > 0)
						{
							return _Levels[0];
						}
						else return null;
					}
				}
				public static string		GetLowestLevel			(string iPath)			
				{
					string[] _Levels = GetLevelsFromPath(iPath);
					{
						if(_Levels.Length > 1)
						{
							/// Parent/Child
							return _Levels[_Levels.Length - 1];
						}
						else return null;
					}
				}
				public static string		TrimRootLevel			(string iPath)			
				{
					string[] iLevels = GetLevelsFromPath(iPath);
					{
						if(iLevels.Length < 2)
						{
							return "";
						}
					}

					string[] oLevels = new string[iLevels.Length - 1];
					{
						for(int Li = 0; Li < oLevels.Length; Li++)
						{
							oLevels[Li] = iLevels[Li + 1];
						}
					}
					return GetPathFromLevels(oLevels);
				}
				public static string		GetValidPath			(string iPath)			
				{
					string[] oPathLevels = GetLevelsFromPath(iPath);
					{
						for(int Li = 0; Li < oPathLevels.Length; Li++)
						{
							if(oPathLevels[Li].StartsWith(AttributeSign.ToString()))
							{
								//Attribute
								continue;
							}
							else
							{
								if(!oPathLevels[Li].Contains(IndexOpener.ToString()) && !oPathLevels[Li].Contains(IndexCloser.ToString()))
								{
									//Element
									oPathLevels[Li] += "[1]";
								}
							}
						}
					}
					return GetPathFromLevels(oPathLevels);
				}

				public static string		GetNodePath				(DataNode iNode)
				{
					Stack<string> _LevelsStack = new Stack<string>();
					{
						DataNode cParent = iNode.Parent;
						{
							_LevelsStack.Push(iNode.ID);

							while(cParent != null)
							{
								_LevelsStack.Push(cParent.ID);

								cParent = cParent.Parent;
							}
						}
					}

					string oNodePath = GetPathFromLevels(_LevelsStack.ToArray());
					return oNodePath;
				}
			//}
			//public class NameWorks						
			//{
				public static string		GetNodeID				(DataNode iNode)						
				{
					if(iNode.XmlNode.NodeType == XmlNodeType.Element)
					{
						int _NodeIndex = GetNodeIndex(iNode);
						{
							return iNode.Name + IndexOpener + _NodeIndex + IndexCloser;
						}
					}
					else if(iNode.XmlNode.NodeType == XmlNodeType.Attribute)
					{
						return AttributeSign + iNode.Name;
					}
					else return iNode.Name;
				}
				public static string		GetNameFromID			(string iNodeID)						
				{
					if(iNodeID.Contains("["))
					{
						return iNodeID.Substring(0, iNodeID.IndexOf("["));
					}
					else return iNodeID;
				}
				public static string		GetUserFriendlyID		(string iNodeID)						
				{
					if(iNodeID.EndsWith("[1]"))
					{
						return iNodeID.Substring(0,iNodeID.Length - 3);
					}
					else return iNodeID;
				}
			
				public static int			GetNodeIndex			(DataNode iNode)						
				{
					if(iNode.Parent == null)
					{
						return 1;
					}

					Dictionary<string, int> _NodeIndices = new Dictionary<string, int>();
					{
						foreach(DataNode cNode in iNode.Parent.Children)
						{
							#region Indices
							if(_NodeIndices.ContainsKey(cNode.Name))
							{
								_NodeIndices[cNode.Name] ++;
							}
							else
							{
								_NodeIndices[cNode.Name] = 1;
							}
							#endregion

							if(cNode.XmlNode == iNode.XmlNode)
							{
								return _NodeIndices[cNode.Name];
							}
						}
					}
					throw new Exception("The node was not found?");
				}
				public static string		GetNamespaceURI			(XmlNode iParent, string iPrefix)		
				{
					return iParent.GetNamespaceOfPrefix(iPrefix);
				}
				public static string		GetPrefix				(string iNodeName)					
				{
					if(iNodeName.Contains(NodeNameDelimiter.ToString()))
					{
						return iNodeName.Split(NodeNameDelimiter)[0];
					}
					else return "";
				}
				public static string		GetLocalName			(string iNodeName)					
				{
					if(iNodeName.Contains(NodeNameDelimiter.ToString()))
					{
						return iNodeName.Split(NodeNameDelimiter)[1];
					}
					else return iNodeName;
				}

				public static XmlNamespaceManager			GetNsManager		(XmlNode iBaseNode, string[] iNsDefinitions)							
				{
					XmlNamespaceManager oNsManager = new XmlNamespaceManager(iBaseNode.OwnerDocument.NameTable);
					{
						Dictionary<string, string> _AllNamespaces = GetAllNamespaces(iBaseNode);
						{
							if(iNsDefinitions.Length > 0 && (iNsDefinitions.Length % 2) == 0)
							{
								//The case when pairs defined: "prefix","NsURI","prefix","NsURI"
								for(int Pi = 0; Pi < iNsDefinitions.Length; Pi += 2)
								{
									string cPrefix	= iNsDefinitions[Pi];
									string cNsURI	= iNsDefinitions[Pi + 1];

									oNsManager.AddNamespace(cPrefix, cNsURI);
								}
							}
							foreach(KeyValuePair<string, string> cDefinition in _AllNamespaces)
							{
								string cPrefix	= cDefinition.Key;
								string cNsURI	= cDefinition.Value;

								if(iNsDefinitions.Length == 1 && cPrefix == "")
								{
									//Setting default NsURI prefix
									cPrefix = iNsDefinitions[0];
								}
								oNsManager.AddNamespace(cPrefix, cNsURI);
							}
						}
					}
					return oNsManager;
				}
				public static Dictionary<string,string>		GetAllNamespaces	(XmlNode iBaseNode)													
				{
					Dictionary<string, string> oNsDict = new Dictionary<string, string>();
					{
						GetAllNamespaces(iBaseNode, ref oNsDict);
					}
					return oNsDict;
				}
				public static void							GetAllNamespaces	(XmlNode iBaseNode, ref Dictionary<string,string> iAllNamespaces)		
				{
					if(iBaseNode.NodeType != XmlNodeType.Element) return;
					
					foreach(XmlAttribute cAttribute in iBaseNode.Attributes)
					{
						if(cAttribute.Name == "xmlns")
						{
							iAllNamespaces[""] = cAttribute.Value;
						}
						else if(cAttribute.Prefix == "xmlns")
						{
							iAllNamespaces[cAttribute.LocalName] = cAttribute.Value;
						}
						else if(cAttribute.Prefix != "")
						{
							iAllNamespaces[cAttribute.Prefix] = cAttribute.NamespaceURI;
						}
					}

					foreach(XmlNode cNode in iBaseNode.ChildNodes)
					{
						GetAllNamespaces(cNode, ref iAllNamespaces);
					}
				}

				
				public static XmlNode		RenameNode				(XmlNode iNode, string iNewName)									
				{
					string _NewPrefix    = GetPrefix(iNewName);
					string _NewLocalName = GetLocalName(iNewName);

					if(_NewPrefix == "xmlns" || _NewLocalName == "xmlns")
					{
						string _NamespaceURI = GetNamespaceURI(iNode.ParentNode, iNewName);
						return RenameNode(iNode, iNewName, _NamespaceURI);
					}
					else
					{
						return RenameNode(iNode, iNewName, iNode.NamespaceURI);
					}
				}
				public static XmlNode		RenameNode				(XmlNode iNode, string iNewName, string iNewNsURI)				
				{
					/**
						This method does not rename nodes in the parent node context.
						Method is used only for the node name predefenition (before the parent node's document context import)
					*/

					XmlNode oNode = iNode.OwnerDocument.CreateNode(iNode.NodeType, iNewName, iNewNsURI);
					{
						if(oNode.NodeType == XmlNodeType.Element)
						{
							foreach(XmlAttribute cAttribute in iNode.Attributes)
							{
								XmlNode cAttribute_Imported = oNode.OwnerDocument.ImportNode(cAttribute, true);
								oNode.Attributes.Append(cAttribute_Imported as XmlAttribute);
							}
							foreach(XmlNode cChild in iNode.ChildNodes)
							{
								XmlNode cChild_Imported = oNode.OwnerDocument.ImportNode(cChild, true);
								oNode.AppendChild(cChild_Imported);
							}
						}
						else
						{
							///Attribute, Text, Comment etc.
							oNode.Value = iNode.Value;
						}
					}
					return oNode;
				}
			//}
			//public static class Creation						
			//{
				public static DataNode		CreateAttribute			(object iValue)								
				{
					DataNode oNode = CreateAttribute(DefaultNodeName, iValue);
					return oNode;
				}
				public static DataNode		CreateAttribute			(string iNodeName, object iValue)				
				{
					DataNode oNode = CreateNode(XmlNodeType.Attribute, iNodeName);
					{
						oNode.Value = iValue.ToString();
					}
					return oNode;
				}
				public static DataNode		CreateElement			()											
				{
					DataNode oNode = CreateElement(DefaultNodeName);
					return oNode;
				}
				public static DataNode		CreateElement			(string iNodeName)							
				{
					DataNode oNode = CreateNode(XmlNodeType.Element, iNodeName);
					return oNode;
				}
				public static DataNode		CreateText				(object iValue)								
				{
					DataNode oNode = CreateNode(XmlNodeType.Text, null);
					{
						oNode.Value = iValue != null ? iValue.ToString() : "";
					}
					return oNode;
				}

				public static DataNode		CreateNode				(XmlNodeType iNodeType)																
				{
					DataNode oNode = CreateNode(iNodeType, DefaultNodeName);
					return oNode;
				}
				public static DataNode		CreateNode				(XmlNodeType iNodeType, string iNodeName)												
				{
					XmlNode oNode = CreateNode(iNodeType, iNodeName, null);
					//DataNode oNode   = CreateNode(oXmlNode);
					return oNode;
				}
				public static DataNode		CreateNode				(XmlNodeType iNodeType, string iNodeName, string iNsURI)								
				{
					DataNode oNode = CreateNode(iNodeType, iNodeName, iNsURI, new XmlDocument());
					return oNode;
				}
				public static DataNode		CreateNode				(XmlNodeType iNodeType, string iNodeName, string iNsURI, XmlDocument iTargetContext)	
				{
					XmlNode _XmlNode = iTargetContext.CreateNode(iNodeType, iNodeName, iNsURI);

					DataNode oNode = CreateNode(_XmlNode);
					return oNode;
				}
				public static DataNode		CreateNode				(XmlNode iXmlNode)																	
				{
					DataNode oNode = new DataNode(iXmlNode);
					return oNode;
				}

				public static XmlNode		AppendAttribute			(XmlNode iTargetNode, XmlNode iAttribute)						
				{
					iAttribute = iTargetNode.OwnerDocument.ImportNode(iAttribute, true);
					iTargetNode.Attributes.Append(iAttribute as XmlAttribute);

					return iAttribute;
				}
				public static XmlNode		AppendElement			(XmlNode iParentNode, XmlNode iChildNode)						
				{
					string _ParentNsURIForChildPrefix = iParentNode.GetNamespaceOfPrefix(iChildNode.Prefix);
					{
						if(iChildNode.NamespaceURI != _ParentNsURIForChildPrefix)
						{
							iChildNode = RenameNode(iChildNode, iChildNode.Name, _ParentNsURIForChildPrefix);
						}
					}
					iChildNode = iParentNode.OwnerDocument.ImportNode(iChildNode, true);

					iParentNode.AppendChild(iChildNode);

					return iChildNode;
				}
				
				public static void			AppendCollection		(DataNode	iNode,	Collection iTargetCollection)			
				{
					AppendCollection(new DataNode[]{iNode}, iTargetCollection);
				}
				public static void			AppendCollection		(DataNode[] iNodes,	Collection iTargetCollection)			
				{
					foreach(DataNode cNode in iNodes)
					{
						if(cNode != null)
						{
							if(iTargetCollection.ItemsType == XmlNodeType.Attribute)
							{
								AppendAttribute(iTargetCollection.Parent.XmlNode, cNode.XmlNode);
							}
							else
							{
								AppendElement(iTargetCollection.Parent.XmlNode, cNode.XmlNode);
							}
						}
					}
				}
			//}
			//public static class Access							
			//{
				public static DataNode		GetNextLevelChild		(string iPath, DataNode iBaseNode, bool iCreateIfMissing)					
				{
					string _NextLevel_ChildID = GetHighestLevel(iPath);
					DataNode oChildNode = GetNodeByExpression(_NextLevel_ChildID, iBaseNode, new string[0]);
					
					if(oChildNode == null && iCreateIfMissing)
					{
						string _NextLevel_ChildName = GetNameFromID(_NextLevel_ChildID);

						oChildNode = CreateElement(_NextLevel_ChildName);
						oChildNode = iBaseNode.Include(oChildNode);
					}
					return oChildNode;
				}
				public static void			SetNodeByID				(DataNode iNewNode, string iNodeID, Collection iNodeCollection)			
				{
					/**
						Assigning node to the specified collection by NodeID.
						Must be called from a collection-defined context
					*/

					
					/*
						- Existing node has been already removed at this point
						- Null-node requires removal of existing node with the same id
					*/
					if(iNewNode != null)
					{
						if
						(
							iNodeCollection.ItemsType == XmlNodeType.Element
							&& iNewNode.Type == XmlNodeType.Text
							///&&			iNewNode.Type != XmlNodeType.Element
						)
						{
							///Exactly value node
							Collection _TargetCollection = iNodeCollection[iNodeID].Children; ///(!!!) ChildNode.Children
							AppendCollection(iNewNode, _TargetCollection);
							return;
						}


						string _NewNode_Name = GetNameFromID(iNodeID);
						string _NewNode_Prefix = GetPrefix(_NewNode_Name);

						
						XmlNodeType _NewNode_Type = (iNodeCollection.ItemsType == XmlNodeType.Attribute) ? XmlNodeType.Attribute : iNewNode.Type;
						string _NewNode_NamespaceURI = (_NewNode_Prefix == "xmlns") ? GetNamespaceURI(iNodeCollection.Parent, _NewNode_Prefix) : null;
						{
							DataNode _NewNode_Alternative = CreateNode
							(
								_NewNode_Type,
								_NewNode_Name,
								_NewNode_NamespaceURI,
								iNodeCollection.Parent.XmlNode.OwnerDocument
							);
							{
								switch(_NewNode_Alternative.Type)
								{
									case XmlNodeType.Attribute:
									{
										_NewNode_Alternative.Name  = _NewNode_Name;
										_NewNode_Alternative.Value = iNewNode.Value;
										break;
									}
									case XmlNodeType.Element:
									{
										_NewNode_Alternative.Name = _NewNode_Name;
										break;
									}
									default:
									{
										_NewNode_Alternative.Value = iNewNode.Value;
										break;
									}
								}
							}
							iNewNode = _NewNode_Alternative;
						}
						AppendCollection(iNewNode, iNodeCollection);
					}
					else
					{
						RemoveNodeByID(iNodeID, iNodeCollection);
					}
				}

				public static void			SetValueForNode			(DataNode iValueNode, DataNode iTargetNode)								
				{
					iTargetNode.Include(iValueNode);
				}
				public static void			RemoveNodeByID			(string iNodeID, Collection iNodeCollection)								
				{
					foreach(DataNode cNode in iNodeCollection)
					{
						if(cNode.ID == iNodeID)
						{
							//Is NsURI assigned?
							if(cNode.Type == XmlNodeType.Attribute)
							{
								iNodeCollection.Parent.XmlNode.Attributes.Remove(cNode.XmlNode as XmlAttribute);
							}
							else
							{
								iNodeCollection.Parent.XmlNode.RemoveChild(cNode.XmlNode);
							}
							break;
						}
					}
				}
				
				public static void			SetNodeByRelativePath	(string iPath, DataNode iBaseNode, DataNode iNewNode)						
				{
					iPath = GetValidPath(iPath);

					if(IsAttributeID(iPath))
					{
						SetNodeByID(iNewNode, iPath.TrimStart(AttributeSign), iBaseNode.Attributes);
						return;
					}
					else
					{
						DataNode cLevel_Child = GetNextLevelChild(iPath, iBaseNode, true);

						string _SubPath = TrimRootLevel(iPath);
						{
							if(_SubPath != "")
							{
								//Direct recursive call
								SetNodeByRelativePath(_SubPath, cLevel_Child, iNewNode);
							}
							else if(iNewNode != null)
							{
								if(iNewNode.Type != XmlNodeType.Attribute && iNewNode.Type != XmlNodeType.Element)
								{
									//Is value node
									AppendElement(cLevel_Child, iNewNode);
								}
							}
							else
							{
								///eq Null -> remove node
								RemoveNodeByID(iPath, iBaseNode.Children);
							}
						}
					}
				}
				public static DataNode		GetNodeByExpression		(string iXPathExpression, DataNode iBaseNode, string[] iNsDefinitions)		
				{
					DataNode[] _SelectedNodes = GetNodesByExpression(iXPathExpression, iBaseNode, iNsDefinitions);
					return (_SelectedNodes.Length > 0) ? _SelectedNodes[0] : null;
				}
				public static DataNode[]	GetNodesByExpression	(string iXPathExpression, DataNode iBaseNode, string[] iNsDefinitions)		
				{
					List<DataNode> oNodes = new List<DataNode>();
					{
						XmlNamespaceManager _XmlNsManager = GetNsManager(iBaseNode.XmlNode, iNsDefinitions);
						XmlNodeList _SelectedNodes = iBaseNode.XmlNode.SelectNodes(iXPathExpression, _XmlNsManager);
						{
							foreach(XmlNode cXmlNode in _SelectedNodes)
							{
								oNodes.Add(new DataNode(cXmlNode));
							}
						}
					}
					return oNodes.ToArray();
				}

				public static void			MergeNodes				(DataNode iBaseNode, DataNode iOtherNode)									
				{
					AppendCollection(iOtherNode.Select("@*"), iBaseNode.Attributes);
					AppendCollection(iOtherNode.Select("*"),  iBaseNode.Children);
				}
			//}
			//public static class Processing						
			//{
				public static DataNode		XslTransform		(DataNode iSrcNode, string iXsltPath)								
				{
					DataNode _XsltNode = DataNode.Load(iXsltPath);

					return XslTransform(iSrcNode, _XsltNode);
				}
				public static DataNode		XslTransform		(DataNode iSrcNode, DataNode iXsltNode)							
				{
					System.Xml.Xsl.XslCompiledTransform _Xslt = new System.Xml.Xsl.XslCompiledTransform();
					{
						_Xslt.Load(iXsltNode.XmlNode);
					}
					return XslTransform(iSrcNode, _Xslt);

				}
				public static DataNode		XslTransform		(DataNode iSrcNode, System.Xml.Xsl.XslCompiledTransform iXslt)					
				{
					throw new Exception("IDataStream members implementation is used instead of StreamFromXmlNode/XmlNodeFromStream");

					//DataNode oNode = new DataNode();
					//{
					//    Stream iStream = new MemoryStream();
					//    Stream oStream = new MemoryStream();
					//    {
					//        DataWriter _Writer = new DataWriter(iStream);
					//        _Writer.Write(iSrcNode);

					//        iStream.Position = 0;
					//    }

					//    XmlReader _XReader = XmlReader.Create(iStream);
					//    XmlWriter _XWriter = XmlWriter.Create(oStream);
					//    {
					//        iXslt.Transform(_XReader, _XWriter);
					//        {
					//            _XWriter.Flush();
					//            oStream.Position = 0;
					//        }
					//    }

					//    DataReader _Reader = new DataReader(oStream);
					//    oNode = _Reader.ReadObject<DataNode>();

					//    iStream.Dispose();
					//    oStream.Dispose();
					//}
					//return oNode;
				}

				public static DataNode		FlattenStructure	(DataNode iNode, bool iInheritAttributes)	
				{
					DataNode oNode = new DataNode(iNode.Name);
					{
						foreach(DataNode cAttribute in iNode.Attributes)
						{
							oNode["@" + cAttribute.Name] = cAttribute.Value;
						}

						DataNode[] _EmptyChildren = iNode.Select(".//*[not(./*)]");
						{
							foreach(DataNode cChild in _EmptyChildren)
							{
								if(iInheritAttributes)
								{
									DataNode cParent = cChild.Parent;
									{
										while(cParent != iNode.Parent)
										{
											foreach(DataNode cAttribute in cParent.Attributes)
											{
												//if(cChild["@" + cAttribute.Name] == null)
												//{
												//    cChild["@" + cAttribute.Name] = cAttribute;
												//}
												
												if(cChild[cAttribute.ID] == null)
												{
													cChild[cAttribute.ID] = cAttribute;
												}
											}
											cParent = cParent.Parent;
										}
									}
								}
								oNode.Include(cChild);
							}
						}
					}
					return oNode;
				}
			//}
			//public static class IO								
			//{
				public static DataNode		ParseXml				(string iXml)							
				{
					DataNode oNode = new DataNode();
					{
						XmlDocument _XDocument = new XmlDocument();
						{
							_XDocument.LoadXml(iXml);
						}
						oNode.XmlNode = _XDocument.DocumentElement;
					}
					return oNode;
				}

				public static DataNode		LoadNode				(string iPath)							
				{
					DataNode oNode = new DataNode();
					{
						XmlDocument _XDocument = new XmlDocument();
						{
							_XDocument.Load(iPath);
							oNode.XmlNode = _XDocument.DocumentElement;
						}

						#region DataNode extensions
						string _DnPrefixNsURI = oNode.XmlNode.GetNamespaceOfPrefix("dn");
						{
							if(_DnPrefixNsURI == "urn:DataNode")
							{
								DataNode[] _NodesToComplete = oNode.Select("//*[@dn:ext]");

								foreach(DataNode cNodeToComplete in _NodesToComplete)
								{
									string cExtPath = System.IO.Path.GetDirectoryName(iPath) + "\\" + cNodeToComplete["@dn:ext"] + DefaultExtension;
									
									DataNode cExtensionNode = DataNode.Load(cExtPath);
									cNodeToComplete.MergeWith(cExtensionNode);
								}
							}
						}
						#endregion
						#region Clearing comments
						foreach(DataNode cNode in oNode.Select("//comment()"))
						{
							if(cNode.Type == XmlNodeType.Comment)
							{
								cNode.Parent.Exclude(cNode);
							}
						}
						#endregion
					}
					return oNode;
				}
				public static void			SaveNode				(DataNode iNode, string iPath)			
				{
					var _WSettings = new XmlWriterSettings();
					{
						_WSettings.Indent = true;
						_WSettings.IndentChars = "\t";
						_WSettings.CloseOutput = true;
					}
					var _Writer = XmlWriter.Create(File.Open(iPath, FileMode.Create), _WSettings);
					{
						iNode.XmlNode.WriteTo(_Writer);

						_Writer.Close();
						
					}
					//_Writer.WriteNode(
					//throw new NotImplementedException();
					//File.WriteAllText(iPath, iNode.Xml);
					
				}

				public static DataNode[]	LoadNodes				(string iPath)							
				{
					return LoadNodes(iPath, null);
				}
				public static DataNode[]	LoadNodes				(string iPath, string iRootName)			
				{
					///throw new NotImplementedException();

					List<DataNode> oNodes = new List<DataNode>();
					{
						if(!Directory.Exists(iPath))
						{
							return new DataNode[0];
						}
						string[] _NodeFiles = Directory.GetFiles(iPath);
						{
							foreach(string cFile in _NodeFiles)
							{
								if(System.IO.Path.GetExtension(cFile) == DefaultExtension)
								{
									DataNode cNode = LoadNode(cFile);
									{
										if(iRootName != null)
										{
											if(cNode.Name == iRootName)
											{
												oNodes.Add(cNode);
											}
											else continue;
										}
										else
										{
											oNodes.Add(cNode);
										}
									}
								}
							}
						}
					}
					return oNodes.ToArray();
				}
			//}
		}

		public static string DefaultExtension									
		{
			get
			{
				return ".xml";
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public XmlNode		XmlNode;
		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public DataNode		Parent												
		{
			get
			{
				if(this.XmlNode.ParentNode != null)
				{
					return new DataNode(this.XmlNode.ParentNode);
				}
				else return null;
			}
		}

		public string		Path												
		{
			get
			{
				return Routines.GetNodePath(this);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public XmlNodeType	Type												
		{
			get
			{
				return this.XmlNode.NodeType;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string		Name												
		{
			get
			{
				return this.XmlNode.Name;
			}
			set
			{
				if(this.XmlNode.Name != value)
				{
					this.XmlNode = Routines.RenameNode(this.XmlNode, value);
				}
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string		ID													
		{
			get
			{
				string oID = Routines.GetNodeID(this);
				{
					oID = Routines.GetUserFriendlyID(oID);
				}
				return oID;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string		Value												
		{
			get
			{
				return this.XmlNode.InnerText;
			}
			set
			{
				this.XmlNode.InnerText = value;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool			IsEmpty												
		{
			get
			{
				return this.Children.Count == 0;
			}
		}

		public string		Xml													
		{
			get
			{
				return this.XmlNode.OuterXml;
			}
		}

		public Collection	Attributes											
		{
			get
			{
				return new Collection(this, XmlNodeType.Attribute);
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public Collection	Children											
		{
			get
			{
				return new Collection(this, XmlNodeType.Element);
			}
		}

		public				DataNode	()															
		{
			this.XmlNode = Routines.CreateElement().XmlNode;
		}
		public				DataNode	(string iNodeName)											
		{
			this.XmlNode = Routines.CreateElement(iNodeName).XmlNode;
		}

		public				DataNode	(string iNodeName, params object[] iPathsAndValues)		
		{
			this.XmlNode = Routines.CreateElement(iNodeName).XmlNode;
			{
				if(iPathsAndValues.Length == 0)
				{
					/// = new DataNode("NodeName");
					return;
				}
				else if(iPathsAndValues.Length == 1)
				{
					/// = new DataNode("NodeName", NodeValue);
					this.Value = iPathsAndValues[0].ToString();
				}
				else if(iPathsAndValues.Length % 2 != 0)
				{
					/// = new DataNode("NodeName", "@name", "Vasya", "@surname");
					throw new Exception("Pairs required! Expected even P-V.Count number");
				}
				else
				{
					/**
						= new DataNode
						(
							"NodeName",

							"@name"				,	"Vasya"				,
							"@surname"			,	"Pupkin"			,
							...
							"Address/@street"	,	"Neizvestnaya"
						);
					*/

					for(int cPoVi = 0; cPoVi < iPathsAndValues.Length; cPoVi += 2)
					{
						if(!iPathsAndValues[cPoVi].ToString().StartsWith("@") && !iPathsAndValues[cPoVi].ToString().Contains("/"))
						{
							throw new Exception("Obsolete AoV pair format specified");
						}
						this[iPathsAndValues[cPoVi].ToString()] = iPathsAndValues[cPoVi + 1].ToString();
					}
				}
			}
		}
		public				DataNode	(XmlNode iBaseNode)											
		{
			this.XmlNode = iBaseNode;
		}
		
		

		public DataNode		this		[string iXPathExpression, params string[] iNsDefinitions]		
		{
			get
			{
				return Routines.GetNodeByExpression(iXPathExpression, this, iNsDefinitions);
			}
			set
			{
				Routines.SetNodeByRelativePath(iXPathExpression, this, value);
			}
		}
		public string		ValueOf		(string iXPathExpression, params string[] iNsDefinitions)		
		{
			DataNode oNode = this[iXPathExpression, iNsDefinitions];
			return oNode.Value;
		}
		public DataNode[]	Select		(string iXPathExpression, params string[] iNsDefinitions)		
		{
			return Routines.GetNodesByExpression(iXPathExpression, this, iNsDefinitions);
		}

		public DataNode		Include			(DataNode iChildNode)									
		{
			XmlNode _XChiCtxImported = Routines.AppendElement(this.XmlNode, iChildNode.XmlNode);
			return new DataNode(_XChiCtxImported);
		}
		public void			Exclude			(DataNode iChildNode)									
		{
			this.XmlNode.RemoveChild(iChildNode.XmlNode);
		}

		public void			IncludeRange	(IEnumerable<DataNode> iChildNodes)						
		{
			foreach(DataNode cNode in iChildNodes)
			{
				this.Include(cNode);
			}
		}
		public void			ExcludeRange	(IEnumerable<DataNode> iChildNodes)						
		{
			foreach(DataNode cNode in iChildNodes)
			{
				this.Exclude(cNode);
			}
		}

		public DataNode		Create		(string iXPathExpression)									
		{
			//The expression "NodeName" will create new or overwrite(!) existing node with path "NodeName[1]"

			this[iXPathExpression] = new DataNode();
			return this[iXPathExpression];
		}
		public DataNode		Create		(string iChildName, params object[] iPathsAndValues)		
		{
			DataNode _NewChild = new DataNode(iChildName, iPathsAndValues);
			return this.Include(_NewChild);
		}
		public void			MergeWith	(DataNode iOtherNode)										
		{
			Routines.MergeNodes(this, iOtherNode);
		}

		public DataNode		Transform	(DataNode iSourceNode)										
		{
			return Routines.XslTransform(iSourceNode, this);
		}

		public static void			Save		(DataNode iNode, string iPath)						
		{
			Routines.SaveNode(iNode, iPath);
		}
		public static DataNode		Load		(string iPath)										
		{
			return Routines.LoadNode(iPath);
		}
		public static DataNode		LoadXml		(string iXml)										
		{
			return Routines.ParseXml(iXml);
		}
		public static DataNode[]	LoadNodes	(string iPath)										
		{
			return Routines.LoadNodes(iPath);
		}
		

		public override string	ToString			()					
		{
			switch(XmlNode.NodeType)
			{
				case XmlNodeType.Attribute: return this.Name + " = \"" + this.Value + "\"";
				case XmlNodeType.Element:
				{
					string oString = "<" + this.Name + "/>";
					{
						int _NodeIndex = Routines.GetNodeIndex(this);
						{
							if(_NodeIndex > 1)
							{
								oString = oString + " - " + _NodeIndex;
							}
						}
					}
					return oString;
				}
				case XmlNodeType.Text: return XmlNode.Value;
				default: return "---";
			}
		}
		public override int		GetHashCode			()					
		{
			return this.XmlNode.GetHashCode();
		}
		public override	bool	Equals				(object iObj)		
		{
			return this == iObj as DataNode;
		}
		

		#region Intefaces
		public IEnumerator<DataNode> GetEnumerator()							
		{
			return this.Children.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()									
		{
			return this.Children.GetEnumerator();
		}
		public int CompareTo(DataNode iOther)									
		{
			return this.Name.CompareTo(iOther.Name);
		}
		public object Clone()													
		{
			return new DataNode(this.XmlNode.Clone());
		}
		//public DataNode Clone(XmlDocument iTargetContext)						
		//{
		//    return new DataNode(this.XmlNode.Clone());
		//}

		#endregion
		#region Conversions

		public static implicit	operator	bool			(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToBoolean(iNode.Value);
			else throw new NullReferenceException();
		}
		public static implicit	operator	byte			(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToByte(iNode.Value);// : new byte();
			else throw new NullReferenceException();
		}
		public static implicit	operator	sbyte			(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToSByte(iNode.Value);// : new sbyte();
			else throw new NullReferenceException();
		}
		public static implicit	operator	int				(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToInt32(iNode.Value);//: new int();
			else throw new NullReferenceException();
		}
		public static implicit	operator	float			(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToSingle(iNode.Value);// : new float();
			else throw new NullReferenceException();
		}
		public static implicit	operator	double			(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToDouble(iNode.Value);// : new double();
			else throw new NullReferenceException();
		}
		public static implicit	operator	string			(DataNode iNode)		
		{
			if(iNode != null) return Convert.ToString(iNode.Value);// : null;
			else return null;//throw new WTFE();
		}
		public static implicit	operator	XmlNode			(DataNode iNode)		
		{
			return iNode != null ? iNode.XmlNode : null;
		}

		public static implicit	operator	DataNode		(bool    iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(byte    iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(sbyte   iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(int     iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(float   iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(double  iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(string  iValue)		
		{
			return Routines.CreateText(iValue);
		}
		public static implicit	operator	DataNode		(XmlNode iValue)		
		{
			return new DataNode(iValue);
		}
		#endregion
		#region Operators
		

		public static	bool operator	==		(DataNode ixOpd, DataNode iyOpd)	
		{
			object _xObj = ixOpd as object;
			object _yObj = iyOpd as object;
			{
				if     (_xObj == _yObj)                return true;
				else if(_xObj == null ^ _yObj == null) return false;
			}
			///Both operands are not a null
			return ixOpd.XmlNode == iyOpd.XmlNode;
		}
		public static	bool operator	==		(DataNode ixOpd, bool iyOpd)		
		{
			return (bool)ixOpd == iyOpd;
		}
		public static	bool operator	==		(DataNode ixOpd, int iyOpd)			
		{
			return (int)ixOpd == iyOpd;
		}
		public static	bool operator	==		(DataNode ixOpd, float iyOpd)		
		{
			return (float)ixOpd == iyOpd;
		}
		public static	bool operator	==		(DataNode ixOpd, double iyOpd)		
		{
			return (double)ixOpd == iyOpd;
		}

		public static	bool operator	!=		(DataNode ixOpd, DataNode iyOpd)	
		{
			return !(ixOpd == iyOpd);
		}
		public static	bool operator	!=		(DataNode ixOpd, bool iyOpd)		
		{
			return !(ixOpd == iyOpd);
		}
		public static	bool operator	!=		(DataNode ixOpd, int iyOpd)			
		{
			return !(ixOpd == iyOpd);
		}
		public static	bool operator	!=		(DataNode ixOpd, float iyOpd)		
		{
			return !(ixOpd == iyOpd);
		}
		public static	bool operator	!=		(DataNode ixOpd, double iyOpd)		
		{
			return !(ixOpd == iyOpd);
		}
		#endregion
	}
}
