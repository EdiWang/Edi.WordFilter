# Edi.WordFilter

Module used in my blog system to filter dirty words (e.g. insulting languages, impertinent words)

It uses Dictionary to filter content very quickly.

![.NET Build Linux](https://github.com/EdiWang/Edi.WordFilter/workflows/.NET%20Build%20Linux/badge.svg)

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/Edi.WordFilter/
[main-nuget-badge]: https://img.shields.io/nuget/v/Edi.WordFilter.svg?style=flat-square&label=nuget

## Usage

**1.Prepare a text file with banned words, for example splitted by "|". Like this:**
```
fuck|shit|ass
```
Because I still want to live, I can't give you the entire keywords list, sorry for that.

**2.Install from NuGet:**
```
Install-Package Edi.WordFilter
```

**3.Use it like this:**
```
var wordFilterDataFilePath = $"{AppDomain.CurrentDomain.GetData(Constants.DataDirectory)}\\BannedWords.txt";
var maskWordFilter = new MaskWordFilter(wordFilterDataFilePath);
username = maskWordFilter.FilterContent(username);
commentContent = maskWordFilter.FilterContent(commentContent);
```

**4.The disharmony words will be replaced by "*"**

### Design Details

Split disharmony word into Dictionary, Key points to the first character, Value points to the next character where Value itself is the Key of the next Dictionary. When filtering content, begin search with the first Dictionary, if matching double side, then it is a disharmony word.

For example, given disharmony word **"FUCK,FS,ABC"**, the following structure is created:

![image](https://raw.githubusercontent.com/EdiWang/Edi.WordFilter/master/img/doc-dic-structure.png)

Each blue box represents a Dictionary, each character of the disharmony word is stored as the Key and pointing to the next Dictionary.

For example, if user input **"FUCK FAKE"**, the flow is:

"F" can be found in the first level of Dictionary(H0), "U" can be found in the Dictionary(H1) where H1's value is reffered to, like this, C and K can be found in H2 and H3, so "FUCK" is a disharmony word.

For the word "FAKE", although "F" can be found in Dictionary(H0), but H0 does not have a value pointing to "A", and "A" also don't have "K" sits in it's value, and "K" also does not exists in the first level of Dictionary, so "FAKE" is not a disharmony word.

## 免责申明

此项目(Edi.WordFilter)及其配套组件均为免费开源的产品，仅用于学习交流，并且不直接向中国提供服务，中国用户请于下载后立即删除。

任何中国境内的组织及个人不得使用此项目(Edi.WordFilter)及其配套组件构建任何形式的面向中国境内用户的网站或服务。

不可用于任何违反中华人民共和国(含台湾省)或使用者所在地区法律法规的用途。

因为作者即本人仅完成代码的开发和开源活动(开源即任何人都可以下载使用)，从未参与用户的任何运营和盈利活动。

且不知晓用户后续将程序源代码用于何种用途，故用户使用过程中所带来的任何法律责任即由用户自己承担。

[《开源软件有漏洞，作者需要负责吗？是的！》](https://go.edi.wang/aka/os251)