﻿using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityUtilityEditor.Configs;

namespace UnityUtilityEditor.CodeGenerating
{
    internal static class LayerSetClassGenerator
    {
        public static string Generate(LayerSetConfig config, SerializedObject tagManager)
        {
            bool needEmptyLine = false;

            const string tab = "    ";
            StringBuilder builder = new StringBuilder();

            GeneratingTools.GenerateBanner(builder);

            builder.AppendLine()
                   .AppendLine("using UnityEngine;")
                   .AppendLine()
                   .Append("namespace ").AppendLine(config.Namespace)
                   .AppendLine("{")
                   .Append(tab).Append("public static class ").AppendLine(config.ClassName)
                   .Append(tab).Append('{').AppendLine();

            if (config.TagFields)
            {
                SerializedProperty tags = tagManager.FindProperty("tags");

                foreach (var item in tags.EnumerateArrayElements())
                {
                    builder.Append(tab)
                           .Append(tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("string ")
                           .Append(item.stringValue.Replace(" ", string.Empty))
                           .Append("Tag")
                           .Append(" = ")
                           .Append('"')
                           .Append(item.stringValue)
                           .Append('"')
                           .Append(';')
                           .AppendLine();
                }

                needEmptyLine = true;
            }

            if (config.SortingLayerFields)
            {
                if (needEmptyLine)
                    builder.AppendLine();

                foreach (var layer in SortingLayer.layers)
                {
                    builder.Append(tab)
                           .Append(tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("int ")
                           .Append(layer.name.Replace(" ", string.Empty))
                           .Append("Id")
                           .Append(" = ")
                           .Append(layer.id)
                           .Append(';')
                           .AppendLine();
                }

                needEmptyLine = true;
            }

            if (config.LayerFields)
            {
                if (needEmptyLine)
                    builder.AppendLine();

                SerializedProperty layers = tagManager.FindProperty("layers");

                foreach (var item in layers.EnumerateArrayElements())
                {
                    if (item.stringValue.IsNullOrWhiteSpace())
                        continue;

                    builder.Append(tab)
                           .Append(tab)
                           .Append("public ")
                           .Append("const ")
                           .Append("int ")
                           .Append(item.stringValue.Replace(" ", string.Empty))
                           .Append("Layer")
                           .Append(" = ")
                           .Append(LayerMask.NameToLayer(item.stringValue))
                           .Append(';')
                           .AppendLine();
                }

                if (config.LayerMasks.Length > 0)
                    builder.AppendLine();

                foreach (var item in config.LayerMasks)
                {
                    builder.Append(tab)
                           .Append(tab)
                           .Append("public ");

                    if (config.MaskFieldType == LayerSetConfig.LayerMaskFieldType.LayerMask)
                    {
                        builder.Append("static ")
                               .Append("readonly ")
                               .Append("LayerMask ");
                    }
                    else
                    {
                        builder.Append("const ")
                               .Append("int ");
                    }

                    builder.Append(item.Name)
                           .Append("Mask")
                           .Append(" = ")
                           .Append(item.Mask)
                           .Append(';')
                           .AppendLine();
                }

                needEmptyLine = true;
            }

            builder.Append(tab).Append('}').AppendLine()
                   .Append('}').AppendLine();

            return builder.ToString();
        }
    }
}