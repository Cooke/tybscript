export type Type =
  | UnionType
  | LiteralType
  | FuncType
  | ObjectType
  | UnknownType
  | NeverType
  | TypeParameter;

export interface UnknownType {
  readonly kind: "Unknown";
}

export interface NeverType {
  readonly kind: "Never";
}

export interface UnionType {
  readonly kind: "Union";
  readonly types: Type[];
}

export interface LiteralType {
  readonly kind: "Literal";
  readonly value: any;
  readonly valueType: Type;
}

export interface FuncType {
  readonly kind: "Func";
  readonly parameters: readonly FuncParameter[];
  readonly returnType: Type;
  // readonly typeParameters?: TypeParameter[];
}

export interface FuncParameter {
  readonly name: string;
  readonly type: Type;
}

export interface ObjectType {
  readonly kind: "Object";
  readonly base: ObjectType | null;
  readonly members: Array<ObjectMember>;
  readonly name: string;
  readonly typeArguments?: Type[];
  readonly typeParameters?: TypeParameter[];
}

export type GenericObjectType = ObjectType & {
  readonly typeParameters: TypeParameter[];
};

export type BoundGenericObjectType = GenericObjectType & {
  readonly typeArguments: Type[];
};

export type ObjectMember = {
  readonly isConst: boolean;
  readonly name: string;
  readonly type: Type;
  readonly typeParameters?: TypeParameter[];
};

export type TypeParameterVariance = "in" | "out" | undefined;

export interface TypeParameter {
  readonly kind: "TypeParameter";
  readonly name: string;
  readonly variance?: TypeParameterVariance;
}
