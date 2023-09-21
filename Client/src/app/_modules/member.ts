import { Photo } from "./Photo";
export interface Member {
  id: number;
  userName: string;
  gender: string;
  dateOfBirth: string;
  knownAs: string;
  photoUrl: string;
  created: string;
  lastActive: string;
  introduction: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;
  photos: Photo[
    
  ];
  age: string;
}

