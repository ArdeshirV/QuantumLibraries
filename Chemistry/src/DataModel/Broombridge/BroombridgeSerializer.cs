﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Microsoft.Quantum.Chemistry.Broombridge
{
    /// <summary>
    /// Enumerable item for Broombridge version numbers.
    /// </summary>
    public enum VersionNumber
    {
        NotRecognized = -1, v0_1 = 0, v0_2 = 1
    }

    /// <summary>
    /// Broombridge deserializers
    /// </summary>
    public static class Deserializers
    {
        /// <summary>
        /// Dictionary from version number strings to version number types.
        /// </summary>
        internal static Dictionary<string, VersionNumber> VersionNumberDict = new Dictionary<string, VersionNumber>()
        {
            // https://github.com/Microsoft/Quantum/blob/master/Chemistry/Schema/broombridge-0.1.schema.json
            {"0.1", VersionNumber.v0_1 },
            {"broombridge-0.1.schema", VersionNumber.v0_1 },
            // TODO: URL of 0.2 schema.
            {"0.2", VersionNumber.v0_2 },
            {"broombridge-0.2.schema", VersionNumber.v0_2 }
        };

        
        /// <summary>
        /// Returns version number of Broombridge file.
        /// </summary>
        /// <param name="filename">Broombridge file address.</param>
        /// <returns>Version number of Broombridge file</returns>
        public static VersionNumber GetVersionNumber(string filename)
        {
            using (var reader = File.OpenText(filename))
            {
                var deserializer = new DeserializerBuilder().Build();
                var data = deserializer.Deserialize<Dictionary<string, object>>(reader);
                var schema = data["$schema"] as string;
                VersionNumber versionNumber = VersionNumber.NotRecognized;
                if(schema != null)
                {
                    foreach (var kv in VersionNumberDict)
                    {
                        if (schema.Contains(kv.Key))
                        {
                            versionNumber = kv.Value;
                            break;
                        }
                    }
                }
                return versionNumber;
            }
        }


        /// <summary>
        /// Returns Broombridge deserialized into the current version data structure.
        /// Data structure is automatically updated to the current Broombridge version.
        /// </summary>
        /// <param name="filename">Broombridge file address.</param>
        /// <returns>Deserializer Broombridge data strauture.</returns>
        public static Data DeserializeBroombridge(string filename)
        {
            /*
            var tmp = GetVersionNumber(filename)
                .Map(
                new[] {
                    (VersionNumber.v0_1, () => DataStructures.Update(DeserializeBroombridgev0_1(filename))),
                    (VersionNumber.v0_2, () => Deserializers.DeserializeBroombridge0_2(filename)),
                    (VersionNumber.NotRecognized, () => System.InvalidOperationException("Unrecognized Broombridge version number."))
                });
        }*/
        
            VersionNumber versionNumber = GetVersionNumber(filename);
            var output = new V0_2.Data();
            if (versionNumber == VersionNumber.v0_1)
            {
                output = DataStructures.Update(DeserializeBroombridgev0_1(filename));
            }
            else if (versionNumber == VersionNumber.v0_2)
            {
                output = DeserializeBroombridgev0_2(filename);
            }
            else
            {
                throw new System.InvalidOperationException("Unrecognized Broombridge version number.");
            }
            return new Data(output);
        }

        /// <summary>
        /// Deserialize Broombridge v0.1 from a file into the Broombridge v0.1 data structure.
        /// </summary>
        /// <param name="filename">Broombridge filename to deserialize</param>
        /// <returns>Deserialized Broombridge v0.1 data.</returns>
        public static V0_1.Data DeserializeBroombridgev0_1(string filename)
        {
            using (var reader = File.OpenText(filename))
            {
                var deserializer = new DeserializerBuilder().Build();
                return deserializer.Deserialize<V0_1.Data>(reader);
            }
        }

        /// <summary>
        /// Deserialize Broombridge v0.2 from a file into the Broombridge v0.2 data structure.
        /// </summary>
        /// <param name="filename">Broombridge filename to deserialize</param>
        /// <returns>Deserialized Broombridge v0.2 data.</returns>
        public static V0_2.Data DeserializeBroombridgev0_2(string filename)
        {
            using (var reader = File.OpenText(filename))
            {
                var deserializer = new DeserializerBuilder().Build();
                return deserializer.Deserialize<V0_2.Data>(reader);
            }
        }
    }

    /// <summary>
    /// Broombridge serializers
    /// </summary>
    public static class Serializers
    {
        /// <summary>
        /// Broombridge serializer
        /// </summary>
        /// <param name="filename">Broombridge filename to serialize</param>
        /// <returns>Serialized Broombridge</returns>
        public static void SerializeBroombridgev0_2(V0_2.Data data, string filename)
        {
            var stringBuilder = new StringBuilder();
            var serializer = new Serializer();
            stringBuilder.AppendLine(serializer.Serialize(data));
            Console.WriteLine(stringBuilder);
            Console.WriteLine("");
        }
    } 
}