﻿using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonUtil.Core;

public class IdiomMatching {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public class Idiom {
        /// <summary>
        /// 深度
        /// </summary>
        private readonly int layer;
        public int Layer {
            get { return layer; }
        }
        /// <summary>
        /// word
        /// </summary>
        private readonly string word;
        public string Word {
            get { return word; }
        }
        /// <summary>
        /// 下一个
        /// </summary>
        public Idiom? Next { get; set; }

        public Idiom(int layer, string word) {
            this.layer = layer;
            this.word = word;
        }

        public Idiom(int layer, string word, Idiom next) {
            this.layer = layer;
            this.word = word;
            this.Next = next;
        }
    }

    private static readonly IReadOnlyCollection<string> idiomList;
    public static IReadOnlyCollection<string> IdiomList {
        get { return idiomList; }
    }

    private static readonly IReadOnlyDictionary<char, List<string>> WordDict;
    private static readonly Random Random = new();
    private static readonly string WordSourcePath = "resource/idioms.txt";
    private static readonly int MaxLayer = 16;

    static IdiomMatching() {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IEnumerable<string> wordList = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                              // 加载失败则退出程序
        try {
            wordList = File.ReadAllLines(WordSourcePath);
        } catch (Exception e) {
            Logger.Fatal(e);
            App.Current.Shutdown();
        }
        var wordDict = new Dictionary<char, List<string>>();
#pragma warning disable CS8604 // Possible null reference argument.
        wordList = wordList.Select(x => x.Trim()).Where(s => s.Any());
        idiomList = new List<string>(wordList);
#pragma warning restore CS8604 // Possible null reference argument.
        foreach (var word in wordList) {
            char c = word.First();
            if (!wordDict.ContainsKey(c)) {
                wordDict[c] = new();
            }
            wordDict[c].Add(word);
        }
        WordDict = wordDict;
    }

    /// <summary>
    /// 获取匹配列表
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static List<List<string>> GetMatchList(string word) {
        word = word.Trim();
        // 找不到
        if (!word.Any() || !WordDict.ContainsKey(word.Last())) {
            return new();
        }
        var matchList = WordDict[word.Last()];
        // 选择后继数最多的
        matchList.Sort((x, y) => {
            var xCount = WordDict.ContainsKey(x.Last()) ? WordDict[x.Last()].Count : 0;
            var yCount = WordDict.ContainsKey(y.Last()) ? WordDict[y.Last()].Count : 0;
            return yCount - xCount;
        });
        var resultList = new List<List<string>>();
        // 查找
        foreach (var item in matchList.GetRange(0, Math.Min(16, matchList.Count))) {
            var idiom = new Idiom(0, item);
            FindNext(idiom, new HashSet<string>());
            var list = new List<string>();
            while (idiom != null) {
                list.Add(idiom.Word);
                idiom = idiom.Next;
            }
            resultList.Add(list);
        }
        return resultList;
    }

    /// <summary>
    /// 查找下一个匹配
    /// </summary>
    /// <param name="root"></param>
    /// <param name="ancestors">已选择的列表</param>
    private static void FindNext(Idiom root, ISet<string> ancestors) {
        if (root.Layer >= MaxLayer) {
            return;
        }
        char childChar = root.Word.Last();
        // 找不到
        if (!WordDict.ContainsKey(childChar)) {
            return;
        }
        // 一级匹配列表，不包含 ancestors 中的词
        var matches = WordDict[childChar].Where(w => !ancestors.Contains(w)).ToList();
        if (!matches.Any()) {
            return;
        }
        // 已经查找过的索引
        var foundNum = new HashSet<int>();
        bool found = false;
        var match = "";
        if (root.Layer + 1 < MaxLayer) {
            // 查找有二级匹配的词
            while (foundNum.Count < matches.Count) {
                int index;
                // 选择未查找的索引
                while (foundNum.Contains(index = Random.Next(matches.Count))) ;
                foundNum.Add(index);
                var tMatch = matches[index]; // 一级匹配
                if (WordDict.ContainsKey(tMatch.Last())) { // 二级匹配
                    found = true;
                    match = tMatch;
                    break;
                }
            }
        }
        // 一级匹配列表所有词都没有后继，则随机选择
        if (!found) {
            match = matches[Random.Next(matches.Count)];
        }
        ancestors.Add(match); // 添加到已选择的列表
        var idiom = new Idiom(root.Layer + 1, match);
        root.Next = idiom;
        FindNext(idiom, ancestors);
    }
}


