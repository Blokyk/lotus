﻿using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[System.Diagnostics.DebuggerDisplay("{name} ({RootNodes.Count} root nodes)")]
public class Graph
{
    protected string name;

    /// <summary>
    /// The name of this graph.
    /// </summary>
    /// <value>A string that represents the name of this graph.</value>
    public string Name {
        get => name;
    }

    protected List<GraphNode> rootNodes;

    /// <summary>
    /// The nodes that are the roots of independent trees.
    /// </summary>
    /// <value>A List of GraphNode containing every root node.</value>
    public List<GraphNode> RootNodes {
        get => rootNodes;
    }

    protected Dictionary<string, string> graphprops;

    public Dictionary<string, string> GraphProps {
        get => graphprops;
    }

    protected Dictionary<string, string> nodeprops;

    public Dictionary<string, string> NodeProps {
        get => nodeprops;
    }

    protected Dictionary<string, string> edgeprops;

    public Dictionary<string, string> EdgeProps {
        get => edgeprops;
    }

    public Graph(string name) {
        this.name = name;
        rootNodes = new List<GraphNode>();
        graphprops = new Dictionary<string, string>();
        nodeprops = new Dictionary<string, string>();
        edgeprops = new Dictionary<string, string>();
    }

    /// <summary>
    /// Adds a new node to this Graph as a root of an independent tree.
    /// </summary>
    /// <param name="node">The GraphNode to add.</param>
    public void AddNode(GraphNode node)
        => rootNodes.Add(node);

    /// <summary>
    /// A representation of this graph in the 'dot' language (https://www.graphviz.org/doc/info/lang.html).
    /// </summary>
    /// <returns>A single string of 'dot' representing this entire graph.</returns>
    public string ToText() {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Append the keyword 'diagraph' followed by the name of the graph, followed, on a new line, by an opening curly bracket
        strBuilder.AppendLine("graph " + name);
        strBuilder.AppendLine("{");

        foreach (var property in graphprops) {
            strBuilder.AppendLine($"\t{property.Key}=\"{property.Value}\"");
        }

        if (nodeprops.Count != 0) {

            strBuilder.AppendLine("\tnode[");

            foreach (var property in nodeprops) {
                strBuilder.AppendLine($"\t\t{property.Key}=\"{property.Value}\"");
            }

            strBuilder.AppendLine("\t]\n");
        }

        if (edgeprops.Count != 0) {

            strBuilder.AppendLine("\tedge [");

            foreach (var property in edgeprops) {
                strBuilder.AppendLine($"\t\t{property.Key}=\"{property.Value}\"");
            }

            strBuilder.AppendLine("\t]");
        }

        strBuilder.AppendLine();

        // A list of GraphNode to keep track of visited nodes
        var registry = new HashSet<GraphNode>();

        // For each independent tree in this graph
        foreach (var node in rootNodes) {
            // Add the root node to the registry
            registry.Add(node);

            // Append the 'dot' representation of this root node to the graph's representation
            strBuilder.AppendLine("\t" + node.ToText(registry));
        }

        // Close the graph by a closing curly bracket on a new line
        strBuilder.AppendLine("\n}");

        // Return the string builder
        return strBuilder.ToString();
    }

    public void AddGraphProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        if (graphprops.ContainsKey(property))
            graphprops[property] = value;
        else
            graphprops.Add(property, value);
    }

    public void AddNodeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        if (nodeprops.ContainsKey(property))
            nodeprops[property] = value;
        else
            nodeprops.Add(property, value);
    }

    public void AddEdgeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        if (edgeprops.ContainsKey(property))
            edgeprops[property] = value;
        else
            edgeprops.Add(property, value);
    }
}

[System.Diagnostics.DebuggerDisplay("{ID}:{Name}")]
public class GraphNode : IEnumerable<GraphNode>
{
    /// <summary>
    /// The unique identifier for this node.
    /// </summary>
    /// <value>A string representing the ID of this node.</value>
    public string ID { get; protected set; }

    /// <summary>
    /// The name of this node.
    /// </summary>
    /// <value>A string representing the name of this node.</value>
    public string Name { get; set; }

    /// <summary>
    /// The children of this node, i.e. the nodes this node points to.
    /// </summary>
    /// <value>A list of GraphNode this node is pointing to, (i.e. children).</value>
    public List<GraphNode> Children { get; protected set; }

    public Dictionary<string, string> Properties { get; protected set; }

    public GraphNode(string name) : this(new Random().Next(), name) { }

    public GraphNode(int id, string text) : this(id.ToString(), "\"" + text + "\"") { }

    public GraphNode(string id, string text) {
        ID = id;
        Name = text;
        Properties = new Dictionary<string, string>();
        Children = new List<GraphNode>();
    }

    /// <summary>
    /// Add a child to this node.
    /// </summary>
    /// <param name="node">The node to add as a child.</param>
    public GraphNode Add(GraphNode node) {
        Children.Add(node);

        return this;
    }

    public GraphNode SetProperty(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return this;

        if (!Properties.TryAdd(property, value)) {
            Properties[property] = value;
        }

        return this;
    }

    public GraphNode SetColor(string color)
        => SetProperty("color", color);

    public GraphNode SetTooltip(string tooltipText)
        => SetProperty("tooltip", tooltipText);

    public string ToText(HashSet<GraphNode> registry) {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Declare the node : Append the id of the node, and set its label to `name`
        strBuilder.Append($"\n\t" + ID + " [label=" + Name);

        if (Properties.Count != 0) {
            //strBuilder.Append("\t" + id + " [");

            foreach (var property in Properties) {
                strBuilder.Append("," + property.Key + "=\"" + property.Value + "\"");
            }

            //strBuilder.Remove(strBuilder.Length - 1, 1);
        }

        strBuilder.AppendLine("]");

        // For each node that is a children of this object
        foreach (var child in Children) {
            // Append the connection of this node (this node's id -> child's id)
            strBuilder.AppendLine("\t" + ID + " -- " + child.ID);

            // Register this child
            registry.Add(child);

            // Then append the representation of the child
            strBuilder.Append("\t" + child.ToText(registry));
        }

        // Return the string builder
        return strBuilder.ToString();
    }

    public IEnumerator<GraphNode> GetEnumerator()
        => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}