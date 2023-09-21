import { User } from "../Interfaces/User";

export class UserParams {
    gender: string;
    maxAge = 100;
    minAge = 18;
    orderby="lastActive"
    pageNumber = 1;
    pageSize = 5;
    constructor(user: User) {
        this.gender = user.gender !== "female" ? "male" : "female"
    }
}