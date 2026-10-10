using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dimension = Autodesk.Revit.DB.Dimension;

namespace MichelleTool
{
    public static class DimensionUtils
    {
        /// <summary>
        /// Phương thức sẽ tổng hợp những reference có thể dim
        /// </summary>
        /// <param name="references">Danh sách các references</param>
        /// <param name="view">View xác định sẽ đặt dim</param>
        /// <param name="lineDim">Đường line dim</param>
        /// <returns></returns>
        public static ReferenceArray CombineReferences(List<Reference> references, Autodesk.Revit.DB.View view, Line lineDim)
        {
            if (!(references?.Count >= 2)) return null;
            if( view == null || lineDim == null) return null;
            // Bước 1: Xác định 1 dim chính trước
            Document doc = view.Document;
            ReferenceArray firstReferenceArr = null;
            Autodesk.Revit.DB.Dimension dimension = null;
            bool checkDim = false;
            int countSegmentNum =-1;
            int countSegmentInput =-1;
            for (int i = 0; i < references.Count; i++)
            {
                for (int j = 0; j < references.Count; j++)
                {
                    if (i == j) continue;
                    var refArr = new ReferenceArray();
                    refArr.Append(references[i]);
                    refArr.Append(references[j]);
                    CommonUtils.RunSafeTransaction(doc, (tx) =>
                    {
                        dimension = view.Document.Create.NewDimension(view, lineDim, refArr);
                        checkDim = dimension.CheckDimensionIsValid();
                        if (checkDim)
                        {
                            firstReferenceArr = refArr;
                            countSegmentNum = dimension.Segments.Size;
                            countSegmentInput = dimension.Segments.Size;
                        }                     
                        tx.RollBack();
                    }, true, false);                   
                    if (checkDim) break;
                }
                if(checkDim) break;
            }
            if(firstReferenceArr==null) return null;
            List<string> stabRefes = new List<string>();
            foreach (Reference refe in firstReferenceArr)
            {
                stabRefes.Add(refe.ConvertToStableRepresentation(doc));
                references.Remove(refe);
            }
            // Bước 2: Xác định các reference thoả mãn dim
            foreach (var item in references)
            {
                var newReferenceArr = new ReferenceArray();
                foreach (Reference refe in firstReferenceArr) newReferenceArr.Append(refe);
                newReferenceArr.Append(item);            
                CommonUtils.RunSafeTransaction(doc, (tx) =>
                {
                    var newDim = doc.Create.NewDimension(view, lineDim, newReferenceArr);
                    checkDim = newDim.CheckDimensionIsValid();
                    if (checkDim)
                    {
                        var num = 0;
                        foreach (DimensionSegment segment in newDim.Segments)
                        {
                            if (!(segment.ValueString == "0" || segment.ValueString.Contains("-"))) num++;
                        }
                        if (!(num == countSegmentNum || num < 2))
                        {
                            string stab = item.ConvertToStableRepresentation(doc);
                            var findCount = stabRefes.Where(k => k == stab).Count();
                            if (findCount <= 0)
                            {
                                countSegmentNum++;
                                firstReferenceArr.Append(item);
                                stabRefes.Add(stab);
                            }
                        }
                        
                    }
                    tx.RollBack();
                }, true, false);             
            }
            return firstReferenceArr;   
        }
        public static ReferenceArray CombineReferencesSubTran(List<Reference> references, Autodesk.Revit.DB.View view, Line lineDim)
        {
            if (!(references?.Count >= 2)) return null;
            if (view == null || lineDim == null) return null;
            // Bước 1: Xác định 1 dim chính trước
            Document doc = view.Document;
            ReferenceArray firstReferenceArr = null;
            Autodesk.Revit.DB.Dimension dimension = null;
            bool checkDim = false;
            int countSegmentNum = -1;
            int countSegmentInput = -1;
            for (int i = 0; i < references.Count; i++)
            {
                for (int j = 0; j < references.Count; j++)
                {
                    if (i == j) continue;
                    var refArr = new ReferenceArray();
                    refArr.Append(references[i]);
                    refArr.Append(references[j]);
                    CommonUtils.RunSafeTransaction(doc, (tx) =>
                    {
                        dimension = view.Document.Create.NewDimension(view, lineDim, refArr);
                        checkDim = dimension.CheckDimensionIsValid();
                        if (checkDim)
                        {
                            firstReferenceArr = refArr;
                            countSegmentNum = dimension.Segments.Size;
                            countSegmentInput = dimension.Segments.Size;
                        }
                        tx.RollBack();
                    }, true, false);
                    if (checkDim) break;
                }
                if (checkDim) break;
            }
            if (firstReferenceArr == null) return null;
            List<string> stabRefes = new List<string>();
            foreach (Reference refe in firstReferenceArr)
            {
                stabRefes.Add(refe.ConvertToStableRepresentation(doc));
                references.Remove(refe);
            }
            // Bước 2: Xác định các reference thoả mãn dim
            foreach (var item in references)
            {
                var newReferenceArr = new ReferenceArray();
                foreach (Reference refe in firstReferenceArr) newReferenceArr.Append(refe);
                newReferenceArr.Append(item);
                CommonUtils.RunSafeTransaction(doc, (tx) =>
                {
                    var newDim = doc.Create.NewDimension(view, lineDim, newReferenceArr);
                    checkDim = newDim.CheckDimensionIsValid();
                    if (checkDim)
                    {
                        var num = 0;
                        foreach (DimensionSegment segment in newDim.Segments)
                        {
                            if (!(segment.ValueString == "0" || segment.ValueString.Contains("-"))) num++;
                        }
                        if (!(num == countSegmentNum || num < 2))
                        {
                            string stab = item.ConvertToStableRepresentation(doc);
                            var findCount = stabRefes.Where(k => k == stab).Count();
                            if (findCount <= 0)
                            {
                                countSegmentNum++;
                                firstReferenceArr.Append(item);
                                stabRefes.Add(stab);
                            }
                        }

                    }
                    tx.RollBack();
                }, true, false);
            }
            return firstReferenceArr;
        }

        public static ReferenceArray FilterReferences(List<Reference> references, Autodesk.Revit.DB.View view, Line lineDim)
        {
            if (!(references?.Count >= 2)) return null;
            if (view == null || lineDim == null) return null;
            // Bước 1: Xác định 1 dim chính trước
            Document doc = view.Document;
            ReferenceArray firstReferenceArr = null;
            bool checkDim = false;
            int countSegmentNum = -1;

            if (firstReferenceArr == null) return null;
            List<string> stabRefes = new List<string>();
            foreach (Reference refe in firstReferenceArr)
            {
                stabRefes.Add(refe.ConvertToStableRepresentation(doc));
                references.Remove(refe);
            }
            // Bước 2: Xác định các reference thoả mãn dim
            foreach (var item in references)
            {
                var newReferenceArr = new ReferenceArray();
                foreach (Reference refe in firstReferenceArr) newReferenceArr.Append(refe);
                newReferenceArr.Append(item);
                var num = 0;
                if (!(num == countSegmentNum || num < 2))
                {
                    string stab = item.ConvertToStableRepresentation(doc);
                    var findCount = stabRefes.Where(k => k == stab).Count();
                    if (findCount <= 0)
                    {
                        countSegmentNum++;
                        firstReferenceArr.Append(item);
                        stabRefes.Add(stab);
                    }
                }
            }
            return firstReferenceArr;
        }

        public static ReferenceArray GetOutLineDimension(List<Reference> references, Autodesk.Revit.DB.View view, Line lineDim)
        {
            if (!(references?.Count >= 2)) return null;
            if (view == null || lineDim == null) return null;
            Document doc = view.Document;
            ReferenceArray refArr = null;
            Autodesk.Revit.DB.Dimension dimension = null;
            bool checkDim = false;
            List<(double, ReferenceArray)> mapSortDimension = new List<(double, ReferenceArray)>();
            for (int i = 0; i < references.Count; i++)
            {
                for (int j = 0; j < references.Count; j++)
                {
                    if (i == j) continue;
                    refArr = new ReferenceArray();
                    refArr.Append(references[i]);
                    refArr.Append(references[j]);
                    CommonUtils.RunSafeTransaction(doc, (tx) =>
                    {
                        dimension = view.Document.Create.NewDimension(view, lineDim, refArr);
                        checkDim = dimension.CheckDimensionIsValid();
                        if (checkDim)
                        {
                            mapSortDimension.Add((Convert.ToDouble(dimension.ValueString), refArr));
                        };
                        tx.RollBack();
                    }, true, false);
                }
            }
            if (mapSortDimension?.Count > 0)
            {
                var sortItems = mapSortDimension.OrderByDescending(k => k.Item1).ToList();
                var itemFirst = sortItems.FirstOrDefault();
                return itemFirst.Item2;
            }
            return null;
        }
    }
}
