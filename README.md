# DeleteRedundantFiles
扫描目录下的空文件夹和冗余目录，并清理

操作说明：

    1、软件支持按钮选择和拖拽文件目录到软件进行添加。

    2、检索后的文件列表，可以双击打开，查看对于的文件情况。

    3、列表支持按住ctrl多选，一起删除。

软件声明：

    1、为防止系统出错，软件不会操作C盘下的文件！

    2、文件操作不可逆，请谨慎使用！

    3、软件操作逻辑如下：

        ①：找到空文件夹后，会继续查找父目录是否为空，如果为空，会一直循环删除。

        ②：找到冗余文件夹后，会继续查找父目录是否为冗余文件，如果是，会一直循环，直到不为冗余文件为止，然后将该文件夹移动到其目录下。

        ③：移动文件到目标位置如果有相同文件夹，会在移动文件夹名称后增加“_去冗余目录。”

        ④：执行冗余文件移动后，其父目录为空，会按照空文件夹逻辑进行删除。

    4、该软件为个人兴趣开发，未做过多测试，请谨慎使用，如在使用中导致文件误删，本人不负法律责任！！！

    5、什么是冗余文件:就是一个文件夹下只有一个文件夹，如此嵌套。除特殊软件需要外，该文件夹分类无特殊意义。
    
    # 本项目使用MPL V2协议开源
