using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using umbraco.presentation.nodeFactory;
using System.Web.Caching;

namespace Sewen.DataType.AutoProperty.Webservices
{
    /// <summary>
    /// Summary description for AutoPropertyService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AutoPropertyService : System.Web.Services.WebService
    {
        const string PRE_CACHE_KEY = "Sewen_AutoProperty_";

        [WebMethod]
        public List<string> getpropertyitems(string PropertyName, int NodeId, string NodeTypeAlias, bool UseParent)
        {
            string cachekey = PRE_CACHE_KEY + PropertyName + "_" + NodeId.ToString() + "_" +NodeTypeAlias;
            if (HttpContext.Current.Cache[cachekey] != null)
            {
                return (List<string>)HttpContext.Current.Cache[cachekey];
            }
            else
            {
                List<string> propertyList = new List<string>();
                Node startNode;

                if (UseParent)
                {
                    startNode = new Node(NodeId).Parent;
                }
                else
                {
                    startNode = new Node(NodeId);
                }

                if (startNode != null)
                {
                    string nodeListCacheKey = PRE_CACHE_KEY + NodeId.ToString() + "_" +NodeTypeAlias;
                    List<Node> nodeList;
                    if (HttpContext.Current.Cache[nodeListCacheKey] != null)
                    {
                        nodeList = (List<Node>)HttpContext.Current.Cache[nodeListCacheKey];
                    }
                    else
                    {
                        nodeList = new List<Node>();
                        GetNodesOfType(startNode, NodeTypeAlias, nodeList);
                        HttpContext.Current.Cache.Add(nodeListCacheKey, nodeList, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.Normal, null);
                    }
                    foreach (Node n in nodeList)
                    {
                        if (n.GetProperty(PropertyName) != null)
                        { 
                            propertyList.Add(n.GetProperty(PropertyName).Value);
                        }
                    }
                }
                propertyList.Sort();

                HttpContext.Current.Cache.Add(cachekey, propertyList.Distinct().ToList(), null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.Normal, null);

                return propertyList.Distinct().ToList();
            }
        }

        private void GetNodesOfType(Node parent, string NodeTypeAlias, List<Node> nodeList)
        {
            foreach (Node child in parent.Children)
            {
                GetNodesOfType(child, NodeTypeAlias, nodeList);
                if (string.IsNullOrEmpty(NodeTypeAlias) || child.NodeTypeAlias == NodeTypeAlias)
                {
                    nodeList.Add(child);
                }
            }
        }
    }
}
