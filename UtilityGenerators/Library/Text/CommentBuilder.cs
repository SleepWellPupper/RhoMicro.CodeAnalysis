namespace RhoMicro.CodeAnalysis.Library.Text;

using System.Runtime.CompilerServices;

#if UTILITYGENERATORS
[IncludeFile]
[NonEquatable]
#endif
internal partial class CommentBuilder(IndentedStringBuilder builder)
{
    public IndentedStringBuilder Builder => builder;
    #region Open Block
    public IndentedStringBuilder OpenSummary() =>
        Builder.OpenBlock(CommentBlocks.Summary);
    public IndentedStringBuilder OpenReturns() =>
        Builder.OpenBlock(CommentBlocks.Returns);
    public IndentedStringBuilder OpenRemarks() =>
        Builder.OpenBlock(CommentBlocks.Remarks);
    public IndentedStringBuilder OpenParam(String name) =>
        Builder.OpenBlock(CommentBlocks.Param(name));
    public IndentedStringBuilder OpenTypeParam(String name) =>
        Builder.OpenBlock(CommentBlocks.TypeParam(name));

    public IndentedStringBuilder OpenList(String type) =>
        Builder.OpenBlock(CommentBlocks.List(type));
    public IndentedStringBuilder OpenItem() =>
        Builder.OpenBlock(CommentBlocks.Item);
    public IndentedStringBuilder OpenTerm() =>
        Builder.OpenBlock(CommentBlocks.Term);
    public IndentedStringBuilder OpenDescription() =>
        Builder.OpenBlock(CommentBlocks.Description);

    public IndentedStringBuilder OpenParagraph() =>
        Builder.OpenBlock(CommentBlocks.Paragraph);
    public IndentedStringBuilder OpenCode() =>
        Builder.OpenBlock(CommentBlocks.Code);
    public IndentedStringBuilder OpenEmphasis() =>
        Builder.OpenBlock(CommentBlocks.Emphasis);
    public IndentedStringBuilder OpenBold() =>
        Builder.OpenBlock(CommentBlocks.Bold);

    public IndentedStringBuilder OpenDocBlock(String name) =>
        Builder.OpenBlock(CommentBlocks.Doc(name));
    public IndentedStringBuilder OpenDocBlock(String name, String attributeName, String attributeValue) =>
        Builder.OpenBlock(CommentBlocks.Doc(name, attributeName, attributeValue));
    public IndentedStringBuilder OpenSingleLineBlock() =>
        Builder.OpenBlock(CommentBlocks.SingleLine);
    public IndentedStringBuilder OpenMultilineBlock() =>
        Builder.OpenBlock(CommentBlocks.Multiline);
    #endregion
    #region Open Block Scope
    public BlockScope OpenSingleLineBlockScope() => new(OpenSingleLineBlock());
    public BlockScope OpenMultilineBlockScope() => new(OpenMultilineBlock());
    public BlockScope OpenDocBlockScope(String name) => new(OpenDocBlock(name));
    public BlockScope OpenDocBlockScope(String name, String attributeName, String attributeValue) =>
        new(OpenDocBlock(name, attributeName, attributeValue));
    #endregion
    #region Self Closing
    public IndentedStringBuilder SeeCRef(String name) =>
        Builder.Append("<see cref=\"").Append(name).Append("\"/>");
    [OverloadResolutionPriority(1)]
    public IndentedStringBuilder SeeCRefMethod(String name, params ReadOnlySpan<String> parameterTypes) => SeeCRefMethod(name, highlightIndex: -1, parameterTypes);
    [OverloadResolutionPriority(1)]
    public IndentedStringBuilder SeeCRefMethod(String name, Int32 highlightIndex, params ReadOnlySpan<String> parameterTypes)
    {
        Builder.Append("<see cref=\"").Append(name).AppendCore('(');

        for(var i = 0; i < parameterTypes.Length; i++)
        {
            if(i > 0)
                Builder.AppendCore(", ");

            if(highlightIndex == i)
                Builder.AppendCore('<');

            Builder.AppendCore(parameterTypes[i]);

            if(highlightIndex == i)
                Builder.AppendCore('>');
        }

        Builder.AppendCore(")\"/>");

        return Builder;
    }
    public IndentedStringBuilder SeeCRefMethod(String name, params IEnumerable<String> parameterTypes) => SeeCRefMethod(name, highlightIndex: -1, parameterTypes);
    public IndentedStringBuilder SeeCRefMethod(String name, Int32 highlightIndex, params IEnumerable<String> parameterTypes)
    {
        Builder.Append("<see cref=\"").Append(name).AppendCore('(');

        var i = 0;
        foreach(var type in parameterTypes)
        {
            if(i > 0)
                Builder.AppendCore(", ");

            if(highlightIndex == i)
                Builder.AppendCore('<');

            Builder.AppendCore(type);

            if(highlightIndex == i)
                Builder.AppendCore('>');

            i++;
        }

        Builder.AppendCore(")\"/>");

        return Builder;
    }

    public IndentedStringBuilder Langword(String name) =>
        Builder.Append("<see langword=\"").Append(name).Append("\"/>");
    public IndentedStringBuilder InheritDoc(String name, Boolean topLevel = true)
    {
        if(topLevel)
            _ = Builder.Append("/// ");

        _ = Builder.Append("<inheritdoc cref=\"").Append(name).Append("\"/>");

        if(topLevel)
            _ = Builder.AppendLine();

        return Builder;
    }
    public IndentedStringBuilder InheritDoc(Boolean topLevel = true)
    {
        if(topLevel)
            _ = Builder.Append("/// ");

        _ = Builder.Append("<inheritdoc/>");

        if(topLevel)
            _ = Builder.AppendLine();

        return Builder;
    }
    public IndentedStringBuilder TypeParamRef(String name) =>
        Builder.Append("<typeparamref name=\"").Append(name).Append("\"/>");
    public IndentedStringBuilder ParamRef(String name) =>
        Builder.Append("<paramref name=\"").Append(name).Append("\"/>");
    public IndentedStringBuilder InternalUse(String name) =>
        Builder.Comment.OpenRemarks()
        .Append("This member is not intended for use by user code inside of or any code outside of ").Comment.SeeCRef(name).Append('.')
        .CloseBlock();
    #endregion
}
