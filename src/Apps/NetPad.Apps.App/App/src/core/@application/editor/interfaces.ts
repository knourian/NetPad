import {DI} from "aurelia";
import {languages} from "monaco-editor";

export const ICompletionItemProvider = DI.createInterface<languages.CompletionItemProvider>();
export const IDocumentSemanticTokensProvider = DI.createInterface<languages.DocumentSemanticTokensProvider>();
export const IDocumentRangeSemanticTokensProvider = DI.createInterface<languages.DocumentRangeSemanticTokensProvider>();