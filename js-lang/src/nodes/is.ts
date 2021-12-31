import { NarrowedSymbol, Symbol } from "../common";
import { booleanType } from "../types/boolean";
import { Type } from "../types/common";
import { ExpressionNode } from "./expression";
import { IdentifierNode } from "./identifier";
import { TypeNode } from "./type";

export class IsNode extends ExpressionNode {
  public get valueType(): Type {
    return booleanType;
  }

  public get truthSymbols(): Symbol[] {
    return this.exp instanceof IdentifierNode && this.exp.symbol!.isConst
      ? [
          new NarrowedSymbol(this.exp.symbol!, (context) => {
            this.type.analyze(context);
            return this.type.type;
          }),
        ]
      : [];
  }

  constructor(
    public readonly exp: ExpressionNode,
    public readonly type: TypeNode
  ) {
    super([exp, type]);
  }
}
