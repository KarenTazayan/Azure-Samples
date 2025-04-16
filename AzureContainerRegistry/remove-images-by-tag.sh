ACR_REGISTRY="acrNAME";
ACR_REPO="shoppingapp/silohost";
IMAGES=("0.0.1-ci.58" "0.0.1-ci.59" "0.0.1-ci.60")

for img in "${IMAGES[@]}"; do
  # Build full reference (repo:tag OR repo@sha256:digest)
  if [[ "$img" == sha256:* ]]; then
    IMAGE="${ACR_REPO}@${img}"
  else
    IMAGE="${ACR_REPO}:${img}"
  fi

  echo "Unlocking $IMAGE in $ACR_REGISTRY …"
  az acr repository update --name "$ACR_REGISTRY" --image "$IMAGE" \
    --delete-enabled true --write-enabled  true --only-show-errors

  echo "Deleting $IMAGE …"
  az acr repository delete --name "$ACR_REGISTRY" --image "$IMAGE" --yes --only-show-errors
done

echo "✅  Done."