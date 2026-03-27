using System.Collections.Generic;
using UnityEngine;

namespace EndlessBeloved.Dialogue
{
    /// <summary>
    /// Parses the chapter JSON format (a flat dictionary of node_id -> node_data)
    /// into a usable dictionary of DialogueNode objects.
    /// </summary>
    public static class DialogueParser
    {
        /// <summary>
        /// Parse a chapter JSON TextAsset into a dictionary of dialogue nodes.
        /// JSON format: { "node_id": { "type": "line", "speaker": "...", ... }, ... }
        /// </summary>
        public static Dictionary<string, DialogueNode> Parse(TextAsset jsonAsset)
        {
            return Parse(jsonAsset.text);
        }

        public static Dictionary<string, DialogueNode> Parse(string json)
        {
            var nodes = new Dictionary<string, DialogueNode>();
            var root = (Dictionary<string, object>)MiniJSON.Deserialize(json);

            if (root == null)
            {
                Debug.LogError("DialogueParser: Failed to parse JSON");
                return nodes;
            }

            foreach (var kvp in root)
            {
                string nodeId = kvp.Key;
                if (kvp.Value is not Dictionary<string, object> nodeData) continue;

                var node = new DialogueNode { id = nodeId };

                node.type = GetString(nodeData, "type", "line");
                node.speaker = GetString(nodeData, "speaker", "narrator");
                node.text = GetString(nodeData, "text", "");
                node.portrait = GetString(nodeData, "portrait", "neutral");
                node.background = GetString(nodeData, "background", "");
                node.music = GetString(nodeData, "music", "");
                node.sfx = GetString(nodeData, "sfx", "");
                node.next = GetString(nodeData, "next", "");
                node.nextScene = GetString(nodeData, "next_scene", "");
                node.cardDrawType = GetString(nodeData, "card_draw_type", "");
                node.forcedCard = GetString(nodeData, "forced_card", "");
                node.minigameId = GetString(nodeData, "minigame_id", "");
                node.onWinNext = GetString(nodeData, "on_win_next", "");
                node.onLoseNext = GetString(nodeData, "on_lose_next", "");

                // Parse choices
                if (nodeData.ContainsKey("choices") && nodeData["choices"] is List<object> choiceList)
                {
                    node.choices = new List<DialogueChoice>();
                    foreach (var choiceObj in choiceList)
                    {
                        if (choiceObj is not Dictionary<string, object> cd) continue;
                        var choice = new DialogueChoice
                        {
                            text = GetString(cd, "text", ""),
                            next = GetString(cd, "next", ""),
                            requiredFlag = GetString(cd, "required_flag", ""),
                            requiredSpell = GetString(cd, "required_spell", ""),
                            requiredAffinityChar = GetString(cd, "required_affinity_char", ""),
                            requiredAffinity = GetInt(cd, "required_affinity", 0)
                        };

                        // Parse affinity changes
                        if (cd.ContainsKey("affinity") && cd["affinity"] is Dictionary<string, object> affDict)
                        {
                            choice.affinity = new Dictionary<string, int>();
                            foreach (var a in affDict)
                                choice.affinity[a.Key] = ToInt(a.Value);
                        }

                        // Parse effects
                        choice.effects = ParseEffects(cd);
                        node.choices.Add(choice);
                    }
                }

                // Parse branch
                if (nodeData.ContainsKey("branch") && nodeData["branch"] is Dictionary<string, object> branchData)
                {
                    node.branch = new DialogueBranch
                    {
                        flag = GetString(branchData, "flag", ""),
                        ifTrue = GetString(branchData, "if_true", ""),
                        ifFalse = GetString(branchData, "if_false", ""),
                        affinityChar = GetString(branchData, "affinity_char", ""),
                        affinityThreshold = GetInt(branchData, "affinity_threshold", 0),
                        ifAbove = GetString(branchData, "if_above", ""),
                        ifBelow = GetString(branchData, "if_below", "")
                    };
                }

                // Parse effects
                node.effects = ParseEffects(nodeData);

                nodes[nodeId] = node;
            }

            return nodes;
        }

        private static List<DialogueEffect> ParseEffects(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("effects") || data["effects"] is not List<object> effectList)
                return null;

            var effects = new List<DialogueEffect>();
            foreach (var eo in effectList)
            {
                if (eo is not Dictionary<string, object> ed) continue;
                effects.Add(new DialogueEffect
                {
                    type = GetString(ed, "type", ""),
                    target = GetString(ed, "target", ""),
                    value = GetString(ed, "value", "")
                });
            }
            return effects;
        }

        private static string GetString(Dictionary<string, object> d, string key, string fallback)
        {
            return d.ContainsKey(key) && d[key] != null ? d[key].ToString() : fallback;
        }

        private static int GetInt(Dictionary<string, object> d, string key, int fallback)
        {
            return d.ContainsKey(key) ? ToInt(d[key]) : fallback;
        }

        private static int ToInt(object o)
        {
            if (o is double d) return (int)d;
            if (o is long l) return (int)l;
            if (o is int i) return i;
            if (int.TryParse(o?.ToString(), out int result)) return result;
            return 0;
        }
    }

    /// <summary>
    /// Minimal JSON deserializer that handles nested dictionaries and lists.
    /// Unity's JsonUtility can't deserialize to Dictionary, so we use this.
    /// </summary>
    public static class MiniJSON
    {
        public static object Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return ParseValue(json.Trim(), ref _pos);
        }

        private static int _pos;

        public static object ParseValue(string json, ref int pos)
        {
            pos = 0;
            return ParseValueInternal(json, ref pos);
        }

        private static object ParseValueInternal(string json, ref int pos)
        {
            SkipWhitespace(json, ref pos);
            if (pos >= json.Length) return null;

            char c = json[pos];
            if (c == '{') return ParseObject(json, ref pos);
            if (c == '[') return ParseArray(json, ref pos);
            if (c == '"') return ParseString(json, ref pos);
            if (c == 't' || c == 'f') return ParseBool(json, ref pos);
            if (c == 'n') { pos += 4; return null; }
            return ParseNumber(json, ref pos);
        }

        private static Dictionary<string, object> ParseObject(string json, ref int pos)
        {
            var dict = new Dictionary<string, object>();
            pos++; // skip {
            SkipWhitespace(json, ref pos);

            while (pos < json.Length && json[pos] != '}')
            {
                string key = ParseString(json, ref pos);
                SkipWhitespace(json, ref pos);
                pos++; // skip :
                SkipWhitespace(json, ref pos);
                object val = ParseValueInternal(json, ref pos);
                dict[key] = val;
                SkipWhitespace(json, ref pos);
                if (pos < json.Length && json[pos] == ',') pos++;
                SkipWhitespace(json, ref pos);
            }
            if (pos < json.Length) pos++; // skip }
            return dict;
        }

        private static List<object> ParseArray(string json, ref int pos)
        {
            var list = new List<object>();
            pos++; // skip [
            SkipWhitespace(json, ref pos);

            while (pos < json.Length && json[pos] != ']')
            {
                list.Add(ParseValueInternal(json, ref pos));
                SkipWhitespace(json, ref pos);
                if (pos < json.Length && json[pos] == ',') pos++;
                SkipWhitespace(json, ref pos);
            }
            if (pos < json.Length) pos++; // skip ]
            return list;
        }

        private static string ParseString(string json, ref int pos)
        {
            pos++; // skip opening "
            int start = pos;
            while (pos < json.Length)
            {
                if (json[pos] == '\\') { pos += 2; continue; }
                if (json[pos] == '"') break;
                pos++;
            }
            string result = json.Substring(start, pos - start);
            result = result.Replace("\\\"", "\"").Replace("\\n", "\n")
                          .Replace("\\t", "\t").Replace("\\\\", "\\");
            pos++; // skip closing "
            return result;
        }

        private static double ParseNumber(string json, ref int pos)
        {
            int start = pos;
            while (pos < json.Length && "0123456789.-+eE".IndexOf(json[pos]) >= 0) pos++;
            double.TryParse(json.Substring(start, pos - start),
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double result);
            return result;
        }

        private static bool ParseBool(string json, ref int pos)
        {
            if (json[pos] == 't') { pos += 4; return true; }
            pos += 5; return false;
        }

        private static void SkipWhitespace(string json, ref int pos)
        {
            while (pos < json.Length && " \t\n\r".IndexOf(json[pos]) >= 0) pos++;
        }
    }
}
