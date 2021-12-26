import { TybscriLexer } from "./generated/TybscriLexer";
import { AnalyzeContext, Node } from "./nodes/base";
import { ExpressionNode } from "./nodes/expression";
import { Type } from "./types/common";

export { TybscriLexer as Lexer };

export interface SourceLocation {
  index: number;
  line: number;
  column: number;
}

export interface SourceSpan {
  readonly start: SourceLocation;
  readonly stop: SourceLocation;
}

export abstract class Symbol {
  constructor(public readonly name: string) {}

  public abstract get valueType(): Type | null;

  public abstract analyze(context: AnalyzeContext): void;
}

export class NarrowedSymbol extends Symbol {
  constructor(
    public readonly outerSymbol: Symbol,
    public readonly valueType: Type
  ) {
    super(outerSymbol.name);
  }

  public analyze(context: AnalyzeContext): void {
    this.outerSymbol.analyze(context);
  }
}

export class SourceSymbol extends Symbol {
  constructor(name: string, public readonly node: Node) {
    super(name);
  }

  public get valueType(): Type | null {
    return this.node.valueType;
  }

  public analyze(context: AnalyzeContext): void {
    this.node.analyze(context);
  }
}

export class ExternalSymbol extends Symbol {
  constructor(name: string, public readonly valueType: Type) {
    super(name);
  }

  public analyze(context: AnalyzeContext): void {}
}

export enum DiagnosticSeverity {
  Error = "error",
}

export interface DiagnosticMessage {
  message: string;
  severity: DiagnosticSeverity;
  span: SourceSpan;
}

export class Scope {
  constructor(
    public readonly parent: Scope | null = null,
    public readonly symbols: Symbol[] = []
  ) {}

  get all(): Symbol[] {
    return [...this.symbols, ...(this.parent?.all ?? [])];
  }

  resolve(name: string): Symbol | null {
    return (
      this.symbols.find((x) => x.name === name) ??
      this.parent?.resolve(name) ??
      null
    );
  }
}
