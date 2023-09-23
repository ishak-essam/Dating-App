export interface Group{
    Name:string;
    Connection:Connection[]
}
export interface Connection{
    connectionId:string;
    username:string;
}