using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;


namespace GIS二次开发
{
    public partial class Frmmain : Form
    {
        //申明窗体层全局变量：
        ILayer pMoveLayer;
        int toIndex;
        //窗体层变量
        IFeatureLayer pFeaLyr;
        //构造函数
        public Frmmain()
        {   
            //初始化窗体
            InitializeComponent();

            //图层控件绑定地图控件
            axTOCControl1.SetBuddyControl(axMapControl_1);
        }

        //窗体加载函数
        private void Frmmain_Load(object sender, EventArgs e) 
        {    //加载地图
            axMapControl_1.LoadMxFile(Application.StartupPath + "\\高世博\\大连海事大学.mxd");
            axMapControl_Eagle.LoadMxFile(Application.StartupPath + "\\高世博\\鹰眼图.mxd");

            //地图比例尺初始化
            axMapControl_1.MapScale = 15000;

            //地图下拉列表比例尺控件信息更新
            repositoryItem_Cbx_Scale.NullText = FunScaleInsertDot();

            //空间信息查询下拉列表图层名
            FunLryname();
        }

        #region 工具栏
        //放大按钮
        private void Btn_ZoomIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand cmd = new ControlsMapZoomInTool();
            cmd.OnCreate(axMapControl_1.Object);
            axMapControl_1.CurrentTool = cmd as ITool;

        }
        //缩小按钮
        private void Btn_ZoomOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand cmd = new ControlsMapZoomOutTool();
            cmd.OnCreate(axMapControl_1.Object);
            axMapControl_1.CurrentTool = cmd as ITool;
        }
        //漫游
        private void Btn_Pan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand cmd = new ControlsMapPanTool();
            cmd.OnCreate(axMapControl_1.Object);
            axMapControl_1.CurrentTool = cmd as ITool;
        }
        //前一视图
        private void Btn_ViewForward_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand cmd = new ControlsMapZoomToLastExtentBackCommand();
            cmd.OnCreate(axMapControl_1.Object);
            axMapControl_1.CurrentTool = cmd as ITool;
        }
        //后一视图
        private void Btn_ViewBack_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand cmd = new ControlsMapZoomToLastExtentForwardCommand();
            cmd.OnCreate(axMapControl_1.Object);
            axMapControl_1.CurrentTool = cmd as ITool;
        }
        //地图刷新
        private void Btn_MapRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            axMapControl_1.Refresh();


        }
        //全图显示
        private void Btn_ViewEntire_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand full = new ControlsMapFullExtentCommand();
            full.OnCreate(axMapControl_1.Object);
            full.OnClick();

        }
        //打开地图
        private void Btn_OpenFileDialog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "打开地图文档";
            openFileDialog.Filter = "地图文档（*.mxd）|*.Mxd|地图模板（*.mxt）|*.Mxt";
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = openFileDialog.FileName;
                axMapControl_1.LoadMxFile(sFileName);

            }

            //空间信息查询下拉列表图层名
            FunLryname();



        }
        //识别查询
        private void Btn_Identify_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { ICommand cmd = new ControlsMapIdentifyTool();
        cmd.OnCreate(axMapControl_1.Object);
        axMapControl_1.CurrentTool = cmd as ITool;
          
        }
        //经典风格风格按钮
        private void Btn_ThemeDefault1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            defaultLookAndFeel1.LookAndFeel.SkinName = "DevExpress Style";
        }
        //时尚风格按钮
        private void Btn_ThemeDeault2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            defaultLookAndFeel1.LookAndFeel.SkinName = "Office 2010 Black";
        }
        //商务风格按钮
        private void Btn_ThemeDefault3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            defaultLookAndFeel1.LookAndFeel.SkinName = "Office 2010 Blue";
        }
        #endregion]

        #region 地图控件事件
        //地图鼠标移动事件
        private void axMapControl_1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //Lbl_XYScale.Caption = "鼠标当前经度：" + e.mapX.ToString("0.######") + ",纬度：" + e.mapY.ToString("0.######") + "。比例尺：1/" + axMapControl_1.MapScale.
            //    ToString("0") + "。";
            double MapX = e.mapX;
            double MapXMin = 60 * (MapX- Math.Floor(MapX));
            double MapxSec = 60 * (MapXMin - Math.Floor(MapXMin));
            double MapY = e.mapY;
            double MapYMin = 60 * (MapY - Math.Floor(MapY));
            double MapYSec = 60 * (MapYMin - Math.Floor(MapYMin));
            Lbl_XY.Caption = "鼠标当前经度：" + MapX.ToString("0") + "°"+MapXMin.ToString("0")+"′"+MapxSec.ToString("0.##")
            +"″,纬度：" + MapY.ToString("0") + "°"+MapYMin.ToString("0")+"′"+MapYSec.ToString("0.##")+"″，比例尺：1/" + axMapControl_1.MapScale.ToString("0") + "。";
        }
      
        //地图范围更新事件
        private void axMapControl_1_OnFullExtentUpdated(object sender, IMapControlEvents2_OnFullExtentUpdatedEvent e)
        {
            //Cbx_Scale显示的地图比例尺更新
            repositoryItem_Cbx_Scale.NullText = FunScaleInsertDot();
        }
        #endregion

        #region 图层控制事件
        //图层移动事件
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button == 1)
            {
                
                esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                object other = null;
                object index = null;
                axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                if (item == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    if (layer is IAnnotationSublayer)   //注记层在表层，不参与移动
                        return;
                    else
                        pMoveLayer = layer;
                }
            }

        }

        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            if (e.button == 1)
            {
                
                esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                object other = null;
                object index = null;
                axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                IMap pMap = axMapControl_1.ActiveView.FocusMap;
                if (item == esriTOCControlItem.esriTOCControlItemLayer || layer != null)
                {
                    //预移动图层和鼠标当前位置图层不一致时
                    if (pMoveLayer != layer)
                    {
                        ILayer pTempLayer;
                        for (int i = 0; i < pMap.LayerCount; i++)
                        {
                            pTempLayer = pMap.get_Layer(i);
                            //获取鼠标当前位置图层的索引值
                            if (pTempLayer == layer)
                            {
                                toIndex = i;
                            }
                        }
                        pMap.MoveLayer(pMoveLayer, toIndex);
                        axMapControl_1.ActiveView.Refresh();
                        axTOCControl1.Update();
                    }
                }
            }

        }

        private void axTOCControl1_OnBeginLabelEdit(object sender, ITOCControlEvents_OnBeginLabelEditEvent e)
        {
            e.canEdit = false;
        }


      

        //比例尺下拉列表变化事件
        private void Cbx_Scale_EditValueChanged(object sender, EventArgs e)
        {
            axMapControl_1.MapScale=Convert.ToDouble(Cbx_Scale.EditValue.ToString());
            axMapControl_1.Refresh();
        }
        #endregion

        #region 自定义函数
        //比例尺字符串插入逗号函数
        string FunScaleInsertDot()
        {
            string strMapScale = axMapControl_1.MapScale.ToString("0");
            int strScale = strMapScale.Length;
            for (int i = 1; (3 * i) < strScale; i++)
            {
                strMapScale = strMapScale.Insert(strMapScale.Length - i * 3 - (i - 1), ",");
            }
            return strMapScale;
        }
               
        //空间信息查询下拉列表图层名
        void FunLryname()
        {  
            Cbx_LyrName.Properties.Items.Clear();
            for (int i = 0; i < axMapControl_1.LayerCount; i++)
            {
                string StrlyrName = axMapControl_1.get_Layer(i).Name;
                this.Cbx_LyrName.Properties.Items.AddRange(new object[] {StrlyrName
            });
                if (i == 0)
                    Cbx_LyrName.Text = StrlyrName;
            }
        }

        //自定义寻找像素单位转地图单位函数ConvertPixelsToMapUnits：
        private double ConvertPixelsToMapUnits(double pixelsUnits)
        {
            IActiveView pActiveView = axMapControl_1.ActiveView;
            int pixelsExtent = pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().right - pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().left;
            double realWorldDisplayExent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double SizeOfOnePixes = realWorldDisplayExent / pixelsExtent;
            return SizeOfOnePixes;
        }

        //自定义FindFeature寻找空间要素函数：
        private IFeature FindFeature(IFeatureLayer pFeatureLayer, ESRI.ArcGIS.Geometry.IPoint point)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            ITopologicalOperator pTopo = (ITopologicalOperator)point;

            //ConvertPixelsToMapUnits()为自定义单位转换函数
            IGeometry pBufferGeo = pTopo.Buffer(ConvertPixelsToMapUnits(10) * 10);
            IEnvelope pBufferEnv = pBufferGeo.Envelope;

            ISpatialFilter pSpatiaFilter = new SpatialFilterClass();
            pSpatiaFilter.Geometry = pBufferEnv;
            pSpatiaFilter.GeometryField = pFeatureClass.ShapeFieldName;

            //获取要素种类
            int iType1 = (int)pFeatureClass.ShapeType;
            switch (iType1)
            {
                case 1:
                    pSpatiaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains; break;
                case 3:
                    pSpatiaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses; break;
                case 4:
                    pSpatiaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects; break;
                default: break;
            }

            IFeatureCursor pFeatCursor;
            pFeatCursor = pFeatureClass.Search(pSpatiaFilter, false);
            IFeature pFeat = pFeatCursor.NextFeature();

            return pFeat;
        }
 #endregion
 
        #region 空间查询定位
         //空间信息查询确定按钮
        private void Btn_QueryOk_Click(object sender, EventArgs e)
        {
            //获得图层ID
            int iLyrID = -1; 
            for (int i = 0; i < axMapControl_1.LayerCount; i++)
            {
                
                if (axMapControl_1.get_Layer(i).Name == Cbx_LyrName.Text)
                {
                    iLyrID = i;
                    break;
                }
            }

            //获取图层信息及要素集
            pFeaLyr = axMapControl_1.get_Layer(iLyrID) as IFeatureLayer;
            IFeatureClass pFC = pFeaLyr.FeatureClass;
            //获得字段的ID
            int iClmID = -1;
            for (int i = 0; i < pFC.Fields.FieldCount;i++ )
            {
                if (pFC.Fields.get_Field(i).Name == "Name" || pFC.Fields.get_Field(i).Name == "name")
                {
                    iClmID = i;
                    break;
                }
               
            }
            //开始查询
            if (iClmID != -1)
            {
                //初始化一个条件过滤器
                IQueryFilter qfilter = new QueryFilter();
                string ColumName = pFC.Fields.get_Field(iClmID).Name;
                qfilter.WhereClause = ColumName + " like'%" + Txt_Query.Text + "%'";
                IFeatureCursor fCursor= pFC.Search(qfilter,false);
            //初始化数据表
            DataTable DT=new DataTable ();
            //数据表字段填充
            for (int i = 0; i < fCursor.Fields.FieldCount; i++)
            {
            DT.Columns.Add(fCursor.Fields.get_Field(i).Name,typeof(string));
            }

                //清除地图选择集
            axMapControl_1.Map.ClearSelection();
                //清除Gridview中的字段，解除gridcontrol数据源
            GridView_Info.Columns.Clear();
            GridControl_Info.DataSource = null;
             //数据填充
            for (int i = 0; i < pFC.FeatureCount(qfilter);i++ )
            {
                IFeature feature = fCursor.NextFeature();
                axMapControl_1.Map.SelectFeature(axMapControl_1.get_Layer(iLyrID), feature);
                DataRow dr = DT.NewRow();
                for (int j = 0; j < feature.Fields.FieldCount; j++)
                {
                    dr[j] = feature.get_Value(j).ToString();
                }
                DT.Rows.Add(dr);
            }
                //绑定结果是
            GridControl_Info.DataSource = DT;
            for (int i = 0; i < GridView_Info.Columns.Count; i++)
           
                if (GridView_Info.Columns[i].Name == "colshape" || GridView_Info.Columns[i].Name == "colShape")
                    GridView_Info.Columns[i].Visible = false;

               
                //地图窗口缩放到选择位置
            IFeatureSelection featureSelection = pFeaLyr as IFeatureSelection;
                if (featureSelection.SelectionSet.Count == 0) return;
                IEnumGeometryBind tEnumGeometryBind = new EnumFeatureGeometryClass();
                tEnumGeometryBind.BindGeometrySource(null, featureSelection.SelectionSet);
                IEnumGeometry tEnumGeometry = (IEnumGeometry)tEnumGeometryBind;
                IGeometryFactory tGeometryFactory = new GeometryEnvironmentClass();
                IGeometry tGeometry = tGeometryFactory.CreateGeometryFromEnumerator(tEnumGeometry);
                //缓冲处理，使处于边界的元素在视图中能够完全显示
                ITopologicalOperator mTopologicalOperator = (ITopologicalOperator)tGeometry;
                IGeometry mPolygonBuffer = mTopologicalOperator.Buffer(0.001) as IGeometry;
                axMapControl_1.Extent = mPolygonBuffer.Envelope;

                axMapControl_1.ActiveView.Refresh();
            }

      
            else
              MessageBox.Show("该图层没有名称字段！！！");
        }
        //空间信息查询ENTER确定按钮
        private void Frmmain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                Btn_QueryOk.PerformClick();//触发button事件  
            }  
        }
        //空间信息查询清除按钮
       private void Btn_QueryClear_Click(object sender, EventArgs e)
        {
             GridControl_Info.DataSource = null;
            axMapControl_1.Map.ClearSelection();
            axMapControl_1.ActiveView.Refresh();
        }
       //GridControl的双击事件
       private void GridControl_Info_DoubleClick(object sender, EventArgs e)
       {
           if (GridControl_Info.DataSource != null)
           { //获取FID或者第一个ID字段名称（索引字段）
               string StrClmIndex = GridView_Info.Columns[0].FieldName;
               //获取FID或者第一个ID字段值
               int IntQueryIndex = Convert.ToInt32(GridView_Info.GetSelectedRows()[0]);

               //查询定位的要素对象
               IFeatureClass pFC = pFeaLyr.FeatureClass;
               IQueryFilter qFilter = new QueryFilter();
               qFilter.WhereClause = StrClmIndex + "=" + IntQueryIndex.ToString();
               IFeatureCursor fCursor = pFC.Search(qFilter, false);
               IFeature feature = fCursor.NextFeature();

               //获取查询要素中心点
               IPoint point = new ESRI.ArcGIS.Geometry.Point();
               //如果是面
               if (feature.Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
               {
                   IArea area = feature.Shape as IArea;
                   point = area.Centroid;
               }
               //如果是点
               else if (feature.Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
               {
                   point = feature.Shape as IPoint;
               }
               //如果是线
               else if (feature.Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
               {
                   IArea area = feature.Extent as IArea;
                   point = area.Centroid;
               }

               //根据要素中心点进行空间定位
               axMapControl_1.CenterAt(point);
               axMapControl_1.Update();
               FlashFeature(feature);
           }
           else
               MessageBox.Show("请先执行查询功能，再进行双击确定定位操作！");
       }

       //闪烁目标
       private void FlashFeature(IFeature iFeature)
       {
           IActiveView iActiveView = axMapControl_1.Map as IActiveView;
           if (iActiveView != null)
           {
               iActiveView.ScreenDisplay.StartDrawing(0, (short)esriScreenCache.esriNoScreenCache);

               //根据几何类型调用不同的过程
               switch (iFeature.Shape.GeometryType)
               {
                   case esriGeometryType.esriGeometryPolyline:
                       FlashLine(axMapControl_1, iActiveView.ScreenDisplay, iFeature.Shape);
                       break;
                   case esriGeometryType.esriGeometryPolygon:
                       FlashPolygon(axMapControl_1, iActiveView.ScreenDisplay, iFeature.Shape);
                       break;
                   case esriGeometryType.esriGeometryPoint:
                       FlashPoint(axMapControl_1, iActiveView.ScreenDisplay, iFeature.Shape);
                       break;
                   default:
                       break;
               }
               iActiveView.ScreenDisplay.FinishDrawing();
           }
       }
       //闪烁线
       private void FlashLine(AxMapControl mapControl, IScreenDisplay iScreenDisplay, IGeometry iGeometry)
       {
           ISimpleLineSymbol iLineSymbol;
           ISymbol iSymbol;
           IRgbColor iRgbColor;

           iLineSymbol = new SimpleLineSymbol();
           iLineSymbol.Width = 16;
           iRgbColor = new RgbColor();
           iRgbColor.Blue = 255;
           iRgbColor.Green = 255;
           iLineSymbol.Color = iRgbColor;
           iSymbol = (ISymbol)iLineSymbol;
           iSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
           mapControl.FlashShape(iGeometry, 3, 500, iSymbol);
       }
       //闪烁面
       private void FlashPolygon(AxMapControl mapControl, IScreenDisplay iScreenDisplay, IGeometry iGeometry)
       {
           ISimpleFillSymbol iFillSymbol;
           ISymbol iSymbol;
           IRgbColor iRgbColor;

           iFillSymbol = new SimpleFillSymbol();
           iFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
           iFillSymbol.Outline.Width = 24;

           iRgbColor = new RgbColor();
           iRgbColor.RGB = System.Drawing.Color.FromArgb(100, 180, 180).ToArgb();
           iFillSymbol.Color = iRgbColor;

           iSymbol = (ISymbol)iFillSymbol;
           iSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
           iScreenDisplay.SetSymbol(iSymbol);
           mapControl.FlashShape(iGeometry, 3, 500, iSymbol);
       }
       //闪烁点
       private void FlashPoint(AxMapControl mapControl, IScreenDisplay iScreenDisplay, IGeometry iGeometry)
       {
           ISimpleMarkerSymbol iMarkerSymbol;
           ISymbol iSymbol;
           IRgbColor iRgbColor;

           iMarkerSymbol = new SimpleMarkerSymbol();
           iMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
           iRgbColor = new RgbColor();
           iRgbColor.Blue = 255;
           iRgbColor.Green = 255;
           iMarkerSymbol.Color = iRgbColor;

           iSymbol = (ISymbol)iMarkerSymbol;
           iSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
           mapControl.FlashShape(iGeometry, 3, 500, iSymbol);
       }
      #endregion
       
        #region 鹰眼图
       private void axMapControl_1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
       {
           //调动鹰眼图
           //定义边界对象
           IEnvelope pEnv = e.newEnvelope as IEnvelope;
           RectangleElement pRectangleEle = new RectangleElement();
           IElement pEle = pRectangleEle as IElement;
           pEle.Geometry = pEnv;

           //定义图像句柄指向鹰眼图
           IGraphicsContainer pGraphicsContainer = axMapControl_Eagle.Map as IGraphicsContainer;
           //获取鹰眼图地图数据的图形容器句柄
           IActiveView pActiveView = pGraphicsContainer as IActiveView;
           pGraphicsContainer.DeleteAllElements();

           //定义颜色
           IRgbColor pColor = new RgbColor();
           pColor.Red = 255;
           //定义线样式
           ILineSymbol pOutline = new SimpleLineSymbol();
           pOutline.Width = 2;
           pOutline.Color = pColor;
           //重新定义颜色
           pColor = new RgbColor();
           pColor.Transparency = 0;
           //定义填充样式
           IFillSymbol pFillSymbol = new SimpleFillSymbol();
           pFillSymbol.Color = pColor;
           pFillSymbol.Outline = pOutline;
           //定义填充元素样式
           IFillShapeElement pFillshapeEle = pEle as IFillShapeElement;
           pFillshapeEle.Symbol = pFillSymbol;
           pEle = pFillshapeEle as IElement;

           //鹰眼图添加元素
           pGraphicsContainer.AddElement(pEle, 0);
           pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
       }
        //鹰眼图鼠标下落事件
       private void axMapControl_Eagle_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
       {
           IPoint Pnt;
           Pnt = new ESRI.ArcGIS.Geometry.Point();
           Pnt.PutCoords(e.mapX, e.mapY);
           axMapControl_1.CenterAt(Pnt);
           axMapControl_1.Refresh();
       }
            //信息查询
       private void Btn_InfoQueryPanel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
       {
           if (DockPanel_Query.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Hidden)
               DockPanel_Query.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
           else
               DockPanel_Query.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
       }
        //鹰眼图
       private void Btn_Eaglepanel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
       {
           panelControl_Eagle.Visible = !panelControl_Eagle.Visible;
       }
        #endregion

        #region 点元素编辑
       //地图鼠标下落事件
       private void axMapControl_1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
       {
           //添加点
           if (axMapControl_1.MousePointer == esriControlsMousePointer.esriPointerCrosshair && axMapControl_1.CurrentTool == null&&e.button
               ==1)
           {
               //获取图层名
               string StrBZLyrName = Cbx_LyrNameEdit.EditValue.ToString();
               //添加点坐标
               IPoint pntAdd = new ESRI.ArcGIS.Geometry.Point();
               pntAdd.X = e.mapX;
               pntAdd.Y = e.mapY;

               //获取查询图层ID
               int iLyr = 0;
               for (int i = 0; i < axMapControl_1.LayerCount; i++)
               {
                   if (axMapControl_1.get_Layer(i).Name == StrBZLyrName)
                   {
                       iLyr = i;
                       break;
                   }
               }

               //得到要添加地物的图层
               IFeatureLayer l = axMapControl_1.Map.get_Layer(iLyr) as IFeatureLayer;

               //获取要素图层的要素类对象
               IFeatureClass fc = l.FeatureClass;

               //定义一个编辑的工作空间                
               IWorkspaceEdit w = (fc as IDataset).Workspace as IWorkspaceEdit;
               IFeatureBuffer f = fc.CreateFeatureBuffer();
               //定义一个插入的要素Cursor                
               IFeatureCursor cur = fc.Insert(true);

               //开始事务操作
               w.StartEditing(false);
               //开始编辑
               w.StartEditOperation();

               //创建一个地物
               f.Shape = pntAdd;
               //插入地物
               cur.InsertFeature(f);

               //结束编辑
               w.StopEditOperation();
               //结束事务操作
               w.StopEditing(true);

               //刷新地图
               IActiveView pActiveView = axMapControl_1.Map as IActiveView;
               pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, axMapControl_1.Map.get_Layer(iLyr), null);
           }
           //删除点
            if (axMapControl_1.MousePointer == esriControlsMousePointer.esriPointerHotLink && axMapControl_1.CurrentTool == null&&e.button==1)
            {
                //获取预标记图层名称
                string StrBZLyrName = Cbx_LyrNameEdit.EditValue.ToString();

                //获取鼠标点击的地图坐标
                IPoint pntDel = new ESRI.ArcGIS.Geometry.Point();
                pntDel.X = e.mapX;
                pntDel.Y = e.mapY;

                //获取编辑图层索引号
                int iLyr = 0;
                for (int i = 0; i < axMapControl_1.LayerCount; i++)
                {
                    if (axMapControl_1.get_Layer(i).Name == StrBZLyrName)
                    {
                        iLyr = i;
                        break;
                    }
                }

                //获取图层FeatureLayer对象
                IFeatureLayer layer = axMapControl_1.Map.get_Layer(iLyr) as IFeatureLayer;

                //FindFeature为寻找空间要素自定义函数
                IFeature feature = FindFeature(layer, pntDel);
                if (feature != null)
                {
                    //获取预删除对象FID
                    string fFID = feature.get_Value(0).ToString();
                    IFeatureClass fc = layer.FeatureClass;
                    IQueryFilter pQueryFilter = new QueryFilterClass();
                    //查询条件为空表示删除所有点
                    pQueryFilter.WhereClause = "fid=" + fFID;
                    ITable pTable = fc as ITable;

                    IWorkspaceEdit w = (fc as IDataset).Workspace as IWorkspaceEdit;
                    //开始事务操作
                    w.StartEditing(false);
                    //开始编辑
                    w.StartEditOperation();
                    pTable.DeleteSearchedRows(pQueryFilter);
                    //结束编辑
                    w.StopEditOperation();
                    //结束事务操作
                    w.StopEditing(true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pQueryFilter);
                    //刷新地图
                    //IActiveView pActiveView = axMapControl_1.Map as IActiveView;
                  //  pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, axMapControl_1.Map.get_Layer(iLyr), null);
                    axMapControl_1.Refresh();
       }
            }
       }

        //添加点
       private void Btn_PntAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
       {

           //指定地图鼠标样式
           axMapControl_1.CurrentTool = null;
           axMapControl_1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
       }

       //删除点
       private void Btn_PntDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
       {
           //指定地图鼠标样式
           axMapControl_1.CurrentTool = null;
           axMapControl_1.MousePointer = esriControlsMousePointer.esriPointerHotLink;
       }
       #endregion


    }


}


        