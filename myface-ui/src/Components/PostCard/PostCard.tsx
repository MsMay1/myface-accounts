import React, {useContext} from "react";
import { Post } from "../../Api/apiClient";
import { Card } from "../Card/Card";
import "./PostCard.scss";
import { Link } from "react-router-dom";
import { LoginContext } from "../../Components/LoginManager/LoginManager";
import { createInteraction, deletePost } from "../../Api/apiClient";


interface PostCardProps {
    post: Post;
}

export function PostCard(props: PostCardProps): JSX.Element {
    const login = useContext(LoginContext);
    return (
        <Card>
            <div className="post-card">
                <img className="image" src={props.post.imageUrl} alt="" />
                <div className="message">{props.post.message}</div>
                <button type="submit"
                onClick={() => {
                    createInteraction(login.username, login.password, { postId: props.post.id, interactionType: 0})}
                    }
                >👍</button> <span>{props.post.likes.length} likes </span>
                <button type="submit"
                onClick={() => {
                    createInteraction(login.username, login.password, { postId: props.post.id, interactionType: 1})}
                    }
                >👎</button> <span>{props.post.dislikes.length} Dislikes </span>
                <button type="submit"
                onClick={() => {
                    deletePost(login.username, login.password, props.post.id)}
                    }
                >delete</button>
                <div className="user">
                    <img className="profile-image" src={props.post.postedBy.profileImageUrl} alt="" />
                    <Link className="user-name" to={`/users/${props.post.postedBy.id}`}>{props.post.postedBy.displayName}</Link>
                </div>
            </div>
        </Card>
    );
}