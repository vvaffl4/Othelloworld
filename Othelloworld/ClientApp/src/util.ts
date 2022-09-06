
export const funcWithType = <T extends object, R>(func: (data: T) => R) =>
  (data: object) =>
    func(data as T);

export const funcWith3Types = <T extends object, R extends object, E extends object, W>(func: (argr1: T, argr2: R, argr3: E) => W) =>
  (arg1: object, arg2: object, arg3: object) =>
    func(arg1 as T, arg2 as R, arg3 as E);