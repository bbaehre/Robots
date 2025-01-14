﻿using System.Drawing;
using Rhino.Geometry;

namespace Robots.Grasshopper;

public class CreateTool : GH_Component
{
    public CreateTool() : base("Create tool", "Tool", "Creates a tool or end effector.", "Robots", "Components") { }
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override Guid ComponentGuid => new("{E59E634B-7AD5-4682-B2C1-F18B73AE05C6}");
    protected override Bitmap Icon => Util.GetIcon("iconCreateTool");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddTextParameter("Name", "N", "Tool name", GH_ParamAccess.item, "DefaultGHTool");
        pManager.AddPlaneParameter("TCP", "P", "TCP plane", GH_ParamAccess.item, Plane.WorldXY);
        pManager.AddPlaneParameter("Calibration", "4", "Optional 4 point TCP calibration. Orient the tool in 4 different ways around the same point in space and input the 4 planes that correspond to the flange", GH_ParamAccess.list);
        pManager.AddNumberParameter("Weight", "W", "Tool weight in kg", GH_ParamAccess.item, 0.0);
        pManager.AddPointParameter("Centroid", "C", "Optional tool center of mass", GH_ParamAccess.item);
        pManager.AddMeshParameter("Mesh", "M", "Tool geometry", GH_ParamAccess.item);
        pManager[2].Optional = true;
        pManager[4].Optional = true;
        pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddParameter(new ToolParameter(), "Tool", "T", "Tool", GH_ParamAccess.item);
        pManager.AddPlaneParameter("TCP", "P", "TCP plane. It might be different from the original if the 4 point calibration is used", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        string? name = null;
        Plane tcp = default;
        double weight = 0;
        Mesh? mesh = null;
        Point3d? centroid = null;
        var planes = new List<Plane>();

        if (!DA.GetData(0, ref name) || name is null) return;
        if (!DA.GetData(1, ref tcp)) return;
        DA.GetDataList(2, planes);
        if (!DA.GetData(3, ref weight)) return; 
        DA.GetData(4, ref centroid);
        DA.GetData(5, ref mesh);

        try
        {
            var tool = new Tool(tcp, name, weight, centroid, mesh, planes);

            DA.SetData(0, tool);
            DA.SetData(1, tool.Tcp);
        }
        catch (ArgumentException e)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
        }
    }
}